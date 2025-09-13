using GeolocationAPI.Domain;

namespace GeolocationAPI.API.Http.DTOs
{
    public class GetBatchStatusResponse
    {
        public Guid Id { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public required int RecordsProcessed { get; set; }
        public required int Successful { get; set; }
        public required int Failed { get; set; }
        public required int TotalRecords { get; set; }
        public required string Status { get; set; }
        public double? ProcessingTime { get; set; }
        public List<BatchItemsDTO> Items { get; set; } = [];

        public static GetBatchStatusResponse FromDomain(BatchProcess batchProcess)
        {
            return new GetBatchStatusResponse
            {
                Id = batchProcess.Id,
                StartedAt = batchProcess.StartedAt,
                CompletedAt = batchProcess.CompletedAt,
                RecordsProcessed = batchProcess.RecordsProcessed,
                TotalRecords = batchProcess.TotalRecords,
                Successful = batchProcess.Successful,
                Failed = batchProcess.Failed,
                Status = batchProcess.Status.ToString(),
                ProcessingTime = batchProcess.ProcessingTime,
                Items = batchProcess.Items.Select(item => new BatchItemsDTO
                {
                    Id = item.Id,
                    BatchProcessId = item.BatchProcessId,
                    IpAddress = item.IpAddress,
                    CountryCode = item.CountryCode,
                    CountryName = item.CountryName,
                    TimeZone = item.TimeZone,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    Status = item.Status.ToString(),
                    ErrorMessage = item.ErrorMessage,
                    ProssessedAt = item.ProssessedAt,
                    ProcessingTime = item.ProcessingTime
                }).ToList()
            };
        }

    }

    public class BatchItemsDTO
    { 
        public Guid Id { get; set; }
        public required Guid BatchProcessId { get; set; }
        public required string IpAddress { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public string? TimeZone { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Status { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? ProssessedAt { get; set; }
        public long? ProcessingTime { get; set; }
    }
}