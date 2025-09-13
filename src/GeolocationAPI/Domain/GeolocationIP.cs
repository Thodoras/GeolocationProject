namespace GeolocationAPI.Domain
{ 
    public class GeolocationIP
    {
        public required IP IP { get; set; }
        public required string CountryCode { get; set; }
        public required string CountryName { get; set; }
        public required string TimeZone { get; set; }
        public required string Latitude { get; set; }
        public required string Longitude { get; set; }
    }
}