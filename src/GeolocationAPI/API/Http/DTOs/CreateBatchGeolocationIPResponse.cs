namespace GeolocationAPI.API.Http.DTOs
{
    public class CreateBatchGeolocationIPResponse
    {
        public Guid ProcessId { get; set; }
        public string StatusUrl { get; set; } = string.Empty;
    }
}