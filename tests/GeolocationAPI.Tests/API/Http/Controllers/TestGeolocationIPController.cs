using Moq;
using GeolocationAPI.Application.GeolocationIP;
using Microsoft.Extensions.Logging;
using GeolocationAPI.API.Http.Controllers;
using GeolocationAPI.Application.BackgroundGeolocationIP;
using Microsoft.AspNetCore.Http;
using GeolocationAPI.API.Http.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GeolocationAPI.Tests.API.Http.Controllers
{
    public class TestGeolocationIPController
    {
        [Fact]
        public async Task TestGeolocationIPService_WhenValid_ThenReturnOk()
        {
            const string testIP = "0.0.0.0";

            var expectedData = new GetGeolocationIPResponse
            {
                IP = testIP,
                CountryName = "Middle Earth",
                CountryCode = "ME",
                TimeZone = "Foo",
                Latitude = "0",
                Longitude = "0"
            };

            var geoIPServiceMock = MockGeoIPService.Create().WithSuccessfulResponse(testIP);
            var geolocationIPService = new GeolocationIPService(
                geoIPServiceMock.Object,
                Mock.Of<ILogger<GeolocationIPService>>()
            );
            var controller = new GeolocationIPController(
                geolocationIPService,
                Mock.Of<IBackgroundGeolocationIPService>(),
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<ILogger<GeolocationIPController>>()
            );

            var actualResult = await controller.GetGeolocationIP(testIP);

            var okResult = Assert.IsType<OkObjectResult>(actualResult);
            var actualData = Assert.IsType<GetGeolocationIPResponse>(okResult.Value);
            Assert.Equal(expectedData.IP, actualData.IP);
            Assert.Equal(expectedData.CountryName, actualData.CountryName);
            Assert.Equal(expectedData.CountryCode, actualData.CountryCode);
            Assert.Equal(expectedData.TimeZone, actualData.TimeZone);
            Assert.Equal(expectedData.Latitude, actualData.Latitude);
            Assert.Equal(expectedData.Longitude, actualData.Longitude);
        }
    }
}
