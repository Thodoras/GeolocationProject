namespace GeolocationAPI.Application.BackgroundGeolocationIP
{
    public class BackgroundGeolocationIPWorker : BackgroundService
    {
        private readonly ILogger<BackgroundGeolocationIPWorker> _logger;

        public BackgroundGeolocationIPWorker(ILogger<BackgroundGeolocationIPWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background worker is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                // Optional recurring logic here
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}