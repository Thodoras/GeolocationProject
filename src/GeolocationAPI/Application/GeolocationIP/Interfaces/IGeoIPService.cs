namespace GeolocationAPI.Application.GeolocationIP.Interfaces
{
    public interface IGeoIPService
    {
        Task<Domain.GeolocationIP> GetIPInfoResponseAsync(Domain.IP ip);
    }
}