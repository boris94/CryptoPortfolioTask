using System.Text.Json.Serialization;

namespace CryptoPortfolio.Models
{
    public class TickersResponse
    {
        [JsonPropertyName("data")]
        public List<CryptoDetails> Data { get; set; }
    }
}
