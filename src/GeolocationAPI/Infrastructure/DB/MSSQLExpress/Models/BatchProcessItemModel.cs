using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GeolocationAPI.Domain;

namespace GeolocationAPI.Infrastructure.DB.MSSQLExpress.Models
{
    public class BatchProcessItemModel
    {
        [Key]
        public Guid Id { get; set; }
        public required Guid BatchProcessId { get; set; }
        public required string IpAddress { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
        public string? TimeZone { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string Status { get; set; } = "Pending";
        public string? ErrorMessage { get; set; }
        public DateTime? ProssessedAt { get; set; }
        public long? ProcessingTime { get; set; }
        [ForeignKey("BatchProcessId")]
        public BatchProcessModel BatchProcess { get; set; } = null!;

        public BatchProcessItem ToDomain()
        {
            return new BatchProcessItem
            {
                Id = this.Id,
                BatchProcessId = this.BatchProcessId,
                IpAddress = this.IpAddress,
                CountryCode = this.CountryCode,
                CountryName = this.CountryName,
                TimeZone = this.TimeZone,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Status = Enum.TryParse<BatchProcessItemStatus>(this.Status, out var status) ? status : BatchProcessItemStatus.Pending,
                ErrorMessage = this.ErrorMessage,
                ProssessedAt = this.ProssessedAt,
                ProcessingTime = this.ProcessingTime
            };
        }

        public static BatchProcessItemModel FromDomain(BatchProcessItem item)
        {
            return new BatchProcessItemModel
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
            };
        }
    }
}