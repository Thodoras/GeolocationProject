using System.Collections.Concurrent;
using GeolocationAPI.Application.BackgroundGeolocationIP.Interfaces;
using GeolocationAPI.Application.GeolocationIP;
using GeolocationAPI.Domain;
using GeolocationAPI.Utils.Exceptions;

namespace GeolocationAPI.Application.BackgroundGeolocationIP
{
    public interface IBackgroundGeolocationIPService
    {
        Task<Guid> StartProcessBatch(List<IP> ipAddresses);
        Task<BatchProcess> GetBatchStatusAsync(Guid processId);
    }

    public class BackgroundGeolocationIPService : IBackgroundGeolocationIPService
    {
        private readonly IGeolocationIPService _geolocationIPService;
        private readonly IBatchProcessRepository _batchProcessRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _maxConcurrency;
        private readonly int _updateInterval;
        private readonly ILogger<BackgroundGeolocationIPService> _logger;
        private readonly SemaphoreSlim _semaphore;

        public BackgroundGeolocationIPService(
            IGeolocationIPService geolocationIPService,
            IBatchProcessRepository batchProcessRepository,
            IServiceScopeFactory serviceScopeFactory,
            int maxConcurrency,
            int updateInterval,
            ILogger<BackgroundGeolocationIPService> logger)

        {
            _geolocationIPService = geolocationIPService;
            _batchProcessRepository = batchProcessRepository;
            _serviceScopeFactory = serviceScopeFactory;
            _maxConcurrency = maxConcurrency;
            _updateInterval = updateInterval;
            _logger = logger;
            _semaphore = new SemaphoreSlim(_maxConcurrency);
        }

        public async Task<Guid> StartProcessBatch(List<IP> ipAddresses)
        {
            await _semaphore.WaitAsync();
            try
            {
                var process = await InitializeBatchProcessAsync(ipAddresses);

                // Use Task.Run to avoid blocking the main thread
                _ = Task.Run(() => ProcessBatchAsync(process.Id, ipAddresses))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            _logger.LogError(t.Exception, "Error processing batch {ProcessId}", process.Id);
                        }
                        _semaphore.Release();
                    }, TaskScheduler.Default);

                _logger.LogInformation("Started background batch process {ProcessId}", process.Id);
                return process.Id;
            }
            catch
            {
                _semaphore.Release();
                throw;
            }
        }

        public async Task<BatchProcess> GetBatchStatusAsync(Guid processId)
        {
            return await _batchProcessRepository.GetDetailedBatchAsync(processId);
        }

        private async Task<BatchProcess> InitializeBatchProcessAsync(List<IP> ipAddresses)
        {
            var process = new BatchProcess
            {
                Id = Guid.NewGuid(),
                RecordsProcessed = 0,
                Successful = 0,
                Failed = 0,
                TotalRecords = ipAddresses.Count,
                Status = BatchProcessStatus.Pending
            };

            return await _batchProcessRepository.CreateBatchAsync(process, ipAddresses);
        }

        private async Task ProcessBatchAsync(Guid processId, List<IP> ipAddresses)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var batchProcessRepository = scope.ServiceProvider.GetRequiredService<IBatchProcessBackgroundRepository>();
                await UpdateBatchProcessToProcessing(processId, batchProcessRepository);
            }

            var processingTimes = new ConcurrentBag<double>();
            var successful = 0;
            var failed = 0;

            var semaphore = new SemaphoreSlim(_maxConcurrency);
            var tasks = new List<Task>();

            foreach (var ipAddress in ipAddresses)
            {
                await semaphore.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using var taskScope = _serviceScopeFactory.CreateScope();
                        var batchProcessItemRepository = taskScope.ServiceProvider.GetRequiredService<IBatchProcessItemBackgroundRepository>();
                        var batchProcessRepository = taskScope.ServiceProvider.GetRequiredService<IBatchProcessBackgroundRepository>();

                        var startTime = DateTime.UtcNow;
                        var succeeded = await ProcessSingleIPAsync(processId, ipAddress, batchProcessItemRepository);
                        var processingTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
                        processingTimes.Add(processingTime);

                        if (succeeded) Interlocked.Increment(ref successful);
                        else Interlocked.Increment(ref failed);

                        if ((successful + failed) % _updateInterval == 0)
                        {
                            await UpdateBatchProcessStatisticsAsync(
                                processId,
                                successful + failed,
                                successful,
                                failed,
                                processingTimes.Average(),
                                false,
                                batchProcessRepository);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing IP {IpAddress} in batch {ProcessId}", ipAddress.Address, processId);
                        Interlocked.Increment(ref failed);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);

            using (var finalScope = _serviceScopeFactory.CreateScope())
            {
                var batchProcessRepository = finalScope.ServiceProvider.GetRequiredService<IBatchProcessBackgroundRepository>();
                await UpdateBatchProcessStatisticsAsync(
                    processId,
                    successful + failed,
                    successful,
                    failed,
                    processingTimes.Any() ? processingTimes.Average() : 0,
                    true,
                    batchProcessRepository);
            }
        }

        private async Task<bool> ProcessSingleIPAsync(
            Guid processId,
            IP ipAddress,
            IBatchProcessItemBackgroundRepository itemRepository)
        {
            var item = new BatchProcessItem
            {
                BatchProcessId = processId,
                IpAddress = ipAddress.Address,
                ProssessedAt = DateTime.UtcNow
            };

            try
            {
                ipAddress.Validate();
                var geoInfo = await _geolocationIPService.GetGeolocationIPAsync(ipAddress);
                item.CountryCode = geoInfo.CountryCode;
                item.CountryName = geoInfo.CountryName;
                item.TimeZone = geoInfo.TimeZone;
                item.Latitude = geoInfo.Latitude;
                item.Longitude = geoInfo.Longitude;
                item.Status = BatchProcessItemStatus.Succeded;
            }
            catch (InvalidIPFormat ex)
            {
                item.Status = BatchProcessItemStatus.Failed;
                item.ErrorMessage = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                item.Status = BatchProcessItemStatus.Failed;
                item.ErrorMessage = ex.Message;
                return false;
            }
            finally
            {
                item.ProcessingTime = (long)(DateTime.UtcNow - item.ProssessedAt.Value).TotalMilliseconds;
                await itemRepository.AddAsync(item);
            }

            return true;
        }

        private async Task UpdateBatchProcessToProcessing(Guid processId, IBatchProcessBackgroundRepository repo)
        {
            var batch = await repo.GetBatchAsync(processId);
            batch.Status = BatchProcessStatus.Processing;
            batch.StartedAt = DateTime.UtcNow;
            await repo.UpdateBatchAsync(batch);
        }

        private async Task UpdateBatchProcessStatisticsAsync(
            Guid processId, int processed, int succeeded, int failed, double avgTime, bool completed,
            IBatchProcessBackgroundRepository repo)
        {
            var batch = await repo.GetBatchAsync(processId);
            batch.RecordsProcessed = processed;
            batch.Successful = succeeded;
            batch.Failed = failed;
            batch.ProcessingTime = avgTime;
            if (completed) batch.Status = BatchProcessStatus.Completed;
            await repo.UpdateBatchAsync(batch);
        }
    }
}