using GeolocationAPI.Domain;
using GeolocationAPI.Application.GeolocationIP.Interfaces;

namespace GeolocationAPI.Application.GeolocationIP
{
    public interface IGeolocationIPService
    {
        Task<Domain.GeolocationIP> GetGeolocationIPAsync(IP ip);
    }
    
    public class GeolocationIPService : IGeolocationIPService
    {
        private readonly IGeoIPRepository _geoIPRepository;
        private readonly ILogger<GeolocationIPService> _logger;

        public GeolocationIPService(
            IGeoIPRepository geoIPRepository,
            ILogger<GeolocationIPService> logger
            )
        {
            _logger = logger;
            _geoIPRepository = geoIPRepository;
        }
        
        public async Task<Domain.GeolocationIP> GetGeolocationIPAsync(IP ip)
        {
            ip.Validate();
            return await _geoIPRepository.GetIPInfoResponseAsync(ip);
        }
    }
}