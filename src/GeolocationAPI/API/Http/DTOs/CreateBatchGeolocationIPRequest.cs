namespace GeolocationAPI.API.Http.DTOs
{
    public class CreateBatchGeolocationIPRequest
    {
        public List<string> IpAddresses { get; set; } = [];
    }
}