using GeolocationAPI.Application.BackgroundGeolocationIP.Interfaces;
using GeolocationAPI.Domain;
using GeolocationAPI.Infrastructure.DB.MSSQLExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace GeolocationAPI.Infrastructure.DB.MSSQLExpress.Repositories
{
    public class BatchProcessRepository : IBatchProcessRepository, IBatchProcessBackgroundRepository
    {
        private readonly GeolocationAPIDbContext _context;
        private readonly ILogger<BatchProcessRepository> _logger;

        public BatchProcessRepository(
            GeolocationAPIDbContext context,
            ILogger<BatchProcessRepository> logger
            )
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BatchProcess> GetBatchAsync(Guid id)
        {
            var resultModel = await _context.BatchProcesses.FindAsync(id);
            if (resultModel == null)
            {
                _logger.LogWarning("BatchProcess with Id {Id} not found.", id);
                throw new KeyNotFoundException($"BatchProcess with Id {id} not found.");
            }

            return resultModel.ToDomain();
        }

        public async Task<BatchProcess> GetDetailedBatchAsync(Guid id)
        {
            var resultModel = await _context.BatchProcesses.
                Include(b => b.Items).
                FirstAsync(b => b.Id == id);
            if (resultModel == null)
            {
                _logger.LogWarning("BatchProcess with Id {Id} not found.", id);
                throw new KeyNotFoundException($"BatchProcess with Id {id} not found.");
            }

            return resultModel.ToDomain();
        }

        public async Task<BatchProcess> CreateBatchAsync(BatchProcess batchProcess, List<IP> ipAddresses)
        {
            var modelBatchProcess = BatchProcessModel.FromDomain(batchProcess);
            _context.BatchProcesses.Add(modelBatchProcess);
            await _context.SaveChangesAsync();
            return modelBatchProcess.ToDomain();
        }

        public async Task UpdateBatchAsync(BatchProcess batchProcess)
        {
            var tracked = await _context.BatchProcesses.
                Include(b => b.Items).
                FirstOrDefaultAsync(b => b.Id == batchProcess.Id);

            if (tracked is null)
                throw new KeyNotFoundException($"Batch with ID {batchProcess.Id} not found.");

            tracked.StartedAt = batchProcess.StartedAt;
            tracked.CompletedAt = batchProcess.CompletedAt;
            tracked.RecordsProcessed = batchProcess.RecordsProcessed;
            tracked.Successful = batchProcess.Successful;
            tracked.Failed = batchProcess.Failed;
            tracked.TotalRecords = batchProcess.TotalRecords;
            tracked.Status = batchProcess.Status.ToString();
            tracked.ProcessingTime = batchProcess.ProcessingTime; 

            await _context.SaveChangesAsync();
        }
    }
}