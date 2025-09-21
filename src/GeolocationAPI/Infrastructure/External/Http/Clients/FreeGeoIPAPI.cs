using GeolocationAPI.Domain;
using GeolocationAPI.Infrastructure.External.Http.ExternalDTOs;
using GeolocationAPI.Application.GeolocationIP.Interfaces;
using GeolocationAPI.Utils.Exceptions;

namespace GeolocationAPI.Infrastructure.External.Http.Clients
{
    public class FreeGeoIPAPI : IGeoIPService
    {
        private readonly string _baseURL;
        private readonly string _apiKey;
        private readonly TimeSpan _minDelayBetweenRequests;
        private readonly HttpClient _httpClient;
        private readonly ILogger<FreeGeoIPAPI> _logger;

        private DateTime _lastRequestTime = DateTime.UtcNow;
        private Object _lovk = new Object();

        public FreeGeoIPAPI(
            string baseURL,
            string apiKey,
            TimeSpan minDelayBetweenRequests,
            HttpClient httpClient,
            ILogger<FreeGeoIPAPI> logger
            )
        {
            _baseURL = baseURL;
            _apiKey = apiKey;
            _minDelayBetweenRequests = minDelayBetweenRequests;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<GeolocationIP> GetIPInfoResponseAsync(IP ip)
        {
            var requestUrl = $"{_baseURL}?apikey={_apiKey}&ip={ip.Address}";
            await DelayIfNeededAsync();
            _logger.LogInformation($"Calling GeoIP API with url: {requestUrl}");
            var response = await _httpClient.GetFromJsonAsync<GetIPInfoResponse>(requestUrl);
            if (response == null)
            {
                _logger.LogError("Failed to get a valid response from GeoIP API.");
                throw new BadGateway("Failed to get a valid response from GeoIP API.");
            }

            return response.ToDomain();
        }

        private async Task DelayIfNeededAsync()
        {
            TimeSpan delayTime;
            lock (_lovk)
            {
                var now = DateTime.UtcNow;
                var timeSinceLastRequest = now - _lastRequestTime;
                if (timeSinceLastRequest < _minDelayBetweenRequests)
                    delayTime = _minDelayBetweenRequests - timeSinceLastRequest;
                else
                    delayTime = TimeSpan.Zero;
                _lastRequestTime = now + delayTime;
            }

            await Task.Delay(delayTime);
        }
    }
}