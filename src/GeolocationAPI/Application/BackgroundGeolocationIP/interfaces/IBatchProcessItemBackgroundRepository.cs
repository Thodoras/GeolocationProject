using GeolocationAPI.Domain;

namespace GeolocationAPI.Application.BackgroundGeolocationIP.Interfaces
{ 
    public interface IBatchProcessItemBackgroundRepository
    {
        Task AddAsync(BatchProcessItem item);
    }
}