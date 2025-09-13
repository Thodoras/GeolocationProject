using System.Text.Json.Serialization;
using GeolocationAPI.Domain;

namespace GeolocationAPI.Infrastructure.External.Http.ExternalDTOs
{
    public class GetIPInfoResponse
    {
        public Data Data { get; set; } = new Data();

        public GeolocationIP ToDomain()
        {
            return new GeolocationIP
            {
                IP = new IP(Data.Ip),
                CountryCode = Data.Location.Country.HascId,
                CountryName = Data.Location.Country.Name,
                TimeZone = Data.Timezone.Code,
                Latitude = Data.Location.Latitude.ToString(),
                Longitude = Data.Location.Longitude.ToString()
            };
        }   
    }

    public class Data
    {
        public string Ip { get; set; }
        public Location Location { get; set; } = new Location();
        public Timezone Timezone { get; set; } = new Timezone();
    }

    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Country Country { get; set; } = new Country();
    }

    public class Country
    {
        public string Name { get; set; }
        [JsonPropertyName("hasc_id")]
        public string HascId { get; set; }
    }

    public class Timezone
    {
        public string Code { get; set; }
    }
}