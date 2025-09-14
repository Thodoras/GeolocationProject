using GeolocationAPI.Application.GeolocationIP.Interfaces;
using GeolocationAPI.Domain;
using Moq;

namespace Tests.GeolocationAPI.Mocks
{
    public static class MockGeoIPService
    {
        public static Mock<IGeoIPService> Create()
        { 
            return new Mock<IGeoIPService>(); 
        }

        public static Mock<IGeoIPService> WithSuccessfulResponse(
            this Mock<IGeoIPService> mock,
            string ipAddress = "0.0.0.0")
        {
            var data = new GeolocationIP
            {
                IP = new IP(ipAddress),
                CountryName = "Middle Earth",
                CountryCode = "ME",
                TimeZone = "Foo",
                Latitude = "0",
                Longitude = "0"
            };

            mock.Setup(x => x.GetIPInfoResponseAsync(It.Is<IP>(ip => ip.Address == ipAddress))).ReturnsAsync(data);
            return mock;
        }
    }
}
