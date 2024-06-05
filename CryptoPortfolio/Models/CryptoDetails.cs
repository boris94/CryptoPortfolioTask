using System.Text.Json.Serialization;

namespace CryptoPortfolio.Models
{
    public class CryptoDetails
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("price_usd")]
        public string Price { get; set; }
    }
}
