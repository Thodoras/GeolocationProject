namespace GeolocationAPI.Application.GeolocationIP.Interfaces
{
    public interface IGeoIPRepository
    {
        Task<Domain.GeolocationIP> GetIPInfoResponseAsync(Domain.IP ip);
    }
}