using GeolocationAPI.Domain;

namespace GeolocationAPI.Application.BackgroundGeolocationIP.Interfaces
{
    public interface IBatchProcessRepository
    {
        Task<BatchProcess> GetDetailedBatchAsync(Guid id);
        Task<BatchProcess> CreateBatchAsync(BatchProcess batchProcess, List<IP> ipAddresses);
    }
}