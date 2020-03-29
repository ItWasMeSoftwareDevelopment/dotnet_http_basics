using Newtonsoft.Json;

namespace ItWasMe.HttpBasics.Models
{
    public class HttpRequestIpDetails
    {
        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }
        [JsonProperty("continentCode")]
        public string ContinentCode { get; set; }
        [JsonProperty("continentName")]
        public string ContinentName { get; set; }
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
        [JsonProperty("countryName")]
        public string CountryName { get; set; }
        [JsonProperty("stateProv")]
        public string State { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
    }
}
