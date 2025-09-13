using GeolocationAPI.Domain;

namespace GeolocationAPI.Application.BackgroundGeolocationIP.Interfaces
{ 
    public interface IBatchProcessBackgroundRepository
    {
        Task<BatchProcess> GetBatchAsync(Guid id);
        Task UpdateBatchAsync(BatchProcess batchProcess);
    }
}