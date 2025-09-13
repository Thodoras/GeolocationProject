namespace GeolocationAPI.Domain
{
    public class BatchProcess
    {
        public Guid Id { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public required int RecordsProcessed { get; set; }
        public required int Successful { get; set; }
        public required int Failed { get; set; }
        public required int TotalRecords { get; set; }
        public required BatchProcessStatus Status { get; set; }
        public double? ProcessingTime { get; set; }
        public List<BatchProcessItem> Items { get; set; } = [];
    }

    public enum BatchProcessStatus
    {
        Pending,
        Processing,
        Completed
    }
}