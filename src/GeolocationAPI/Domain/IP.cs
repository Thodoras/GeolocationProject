using GeolocationAPI.Utils.Exceptions;

namespace GeolocationAPI.Domain
{
    public class IP
    {
        public string Address { get; set; }

        public IP(string address)
        {
            Address = address;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Address))
                throw new InvalidIPFormat("IP address cannot be null or empty.");

            var parts = Address.Split('.');
            if (parts.Length != 4)
                throw new InvalidIPFormat("IP address must have four octets.");

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int num) || num < 0 || num > 255)
                    throw new InvalidIPFormat($"Invalid octet '{part}' in IP address.");
            }
        }
    }
}