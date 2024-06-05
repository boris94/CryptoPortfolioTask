using System.Text.Json.Serialization;

namespace CryptoPortfolio.Models
{
    public class CryptoGlobalResponse
    {
        [JsonPropertyName("coins_count")]
        public int CoinsCount { get; set; }
    }
}
