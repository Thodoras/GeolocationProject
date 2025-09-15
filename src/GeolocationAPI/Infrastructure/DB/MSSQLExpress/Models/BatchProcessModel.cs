using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GeolocationAPI.Domain;

namespace GeolocationAPI.Infrastructure.DB.MSSQLExpress.Models
{
    public class BatchProcessModel
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public required int RecordsProcessed { get; set; }
        public required string Status { get; set; }
        public double? ProcessingTime { get; set; }
        public List<BatchProcessItemModel> Items { get; set; } = [];
        [NotMapped]
        public int Successful => Items.Count(i => i.Status == "Success");
        [NotMapped]
        public int Failed => Items.Count(i => i.Status == "Failed");
        [NotMapped]
        public int TotalRecords => Items.Count(i => i.Status != "Pending");

        public static BatchProcessModel FromDomain(BatchProcess batchProcess)
        {
            return new BatchProcessModel
            {
                Id = batchProcess.Id,
                StartedAt = batchProcess.StartedAt,
                CompletedAt = batchProcess.CompletedAt,
                RecordsProcessed = batchProcess.RecordsProcessed,
                Status = batchProcess.Status.ToString(),
                ProcessingTime = batchProcess.ProcessingTime,
            };
        }

        public BatchProcess ToDomain()
        {
            return new BatchProcess
            {
                Id = this.Id,
                StartedAt = this.StartedAt,
                CompletedAt = this.CompletedAt,
                RecordsProcessed = this.RecordsProcessed,
                TotalRecords = this.TotalRecords,
                Successful = this.Successful,
                Failed = this.Failed,
                Status = Enum.TryParse<BatchProcessStatus>(this.Status, out var status) ? status : BatchProcessStatus.Pending,
                ProcessingTime = this.ProcessingTime,
                Items = this.Items.Select(item => item.ToDomain()).ToList()
            };
        }
 
    }
}