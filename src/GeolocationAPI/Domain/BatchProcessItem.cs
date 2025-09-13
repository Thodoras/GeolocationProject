namespace GeolocationAPI.Domain
{
    public class BatchProcessItem
    {
        public Guid Id { get; set; }
        public required Guid BatchProcessId { get; set; }
        public required string IpAddress { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public string? TimeZone { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public BatchProcessItemStatus Status { get; set; } = BatchProcessItemStatus.Pending;
        public string? ErrorMessage { get; set; }
        public DateTime? ProssessedAt { get; set; }
        public long? ProcessingTime { get; set; }
        public BatchProcess BatchProcess { get; set; } = null!;
    }

    public enum BatchProcessItemStatus
    {
        Pending,
        Succeded,
        Failed
    }
}