using GeolocationAPI.Domain;
using GeolocationAPI.Utils.Exceptions;

namespace GeolocationAPI.Tests.Domain
{
    public class IPTests
    {
        [Theory]
        [InlineData("192.168.1.1")]
        [InlineData("8.8.8.8")]
        [InlineData("0.0.0.0")]
        [InlineData("127.0.0.1")]
        public void TestValidate_WhenValidIP_ThenDoesNotThrow(string validIP)
        {
            var ip = new IP(validIP);
            var exception = Record.Exception(() => ip.Validate());
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void TestValidate_WhenEmptyOrNullIP_ThenThrowsInvalidIPFormat(string invalidIP)
        {
            var ip = new IP(invalidIP);
            var exception = Assert.Throws<InvalidIPFormat>(() => ip.Validate());
            Assert.Equal("IP address cannot be null or empty.", exception.Message);
        }

        [Theory]
        [InlineData("192.168.1")]
        [InlineData("192.168.1.1.1")]
        [InlineData("192.168")]
        [InlineData("192")]
        public void TestValidate_WhenIncorrectNumberOfOctets_ThenThrowsInvalidIPFormat(string invalidIP)
        {
            var ip = new IP(invalidIP);
            var exception = Assert.Throws<InvalidIPFormat>(() => ip.Validate());
            Assert.Equal("IP address must have four octets.", exception.Message);
        }

        [Theory]
        [InlineData("256.100.50.25")]
        [InlineData("192.168.-1.1")]
        [InlineData("192.168.1.999")]
        public void TestValidate_WhenOctetOutOfRange_ThenThrowsInvalidIPFormat(string invalidIP)
        {
            var ip = new IP(invalidIP);
            var exception = Assert.Throws<InvalidIPFormat>(() => ip.Validate());
            Assert.Contains("Invalid octet", exception.Message);
        }

        [Theory]
        [InlineData("192.168.one.1")]
        [InlineData("abc.def.ghi.jkl")]
        [InlineData("192.168.1.two")]
        public void TestValidate_WhenNonNumericOctet_ThenThrowsInvalidIPFormat(string invalidIP)
        {
            var ip = new IP(invalidIP);
            var exception = Assert.Throws<InvalidIPFormat>(() => ip.Validate());
            Assert.Contains("Invalid octet", exception.Message);
        }       
    }
}