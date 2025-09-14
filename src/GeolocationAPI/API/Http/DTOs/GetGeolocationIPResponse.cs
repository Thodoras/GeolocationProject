using GeolocationAPI.Domain;

namespace GeolocationAPI.API.Http.DTOs
{
    public class GetGeolocationIPResponse
    {
        public string IP { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string TimeZone { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public GetGeolocationIPResponse() { }

        public GetGeolocationIPResponse(GeolocationIP geolocationIP)
        {
            IP = geolocationIP.IP.Address;
            CountryCode = geolocationIP.CountryCode;
            CountryName = geolocationIP.CountryName;
            TimeZone = geolocationIP.TimeZone;
            Latitude = geolocationIP.Latitude;
            Longitude = geolocationIP.Longitude;
        }
    }
}