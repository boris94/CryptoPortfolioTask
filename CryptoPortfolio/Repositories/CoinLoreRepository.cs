using CryptoPortfolio.Data;
using CryptoPortfolio.Models;
using CryptoPortfolio.Repositories.Interfaces;
using System.Text.Json;

namespace CryptoPortfolio.Repositories
{
    public class CoinLoreRepository : ICryptoRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CoinLoreRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Dictionary<string, decimal>> GetNewPrices(IEnumerable<string> currenciesIds)
        {
            var newPrices = new Dictionary<string, decimal>();
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var url = $"{ApiUrls.COIN_LORE_TICKER_SPECIFIC}?id={string.Join(",", currenciesIds)}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await httpClient.SendAsync(request);

                if (response != null && response.IsSuccessStatusCode)
                {
                    List<CryptoDetails>? result;
                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        result = await JsonSerializer.DeserializeAsync<List<CryptoDetails>>(contentStream);
                    }

                    if (result != null)
                    {
                        newPrices = result.ToDictionary(x => x.Symbol, x => decimal.Parse(x.Price));
                    }
                }
            }

            return newPrices;
        }

        public async Task<int> GetTotalCoinsCount()
        {
            int totalCoins = 0;
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, ApiUrls.COIN_LORE_GLOBAL);
                var response = await httpClient.SendAsync(request);

                if (response != null && response.IsSuccessStatusCode)
                {
                    List<CryptoGlobalResponse>? result;
                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        result = await JsonSerializer.DeserializeAsync<List<CryptoGlobalResponse>>(contentStream);
                    }

                    if (result != null && result.Count > 0)
                    {
                        totalCoins = result.First().CoinsCount;
                    }
                }
            }

            return totalCoins;
        }

        public async Task<List<CryptoDetails>> GetTickersData(int start, int limit)
        {
            var cryptoDetails = new List<CryptoDetails>();
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var url = $"{ApiUrls.COIN_LORE_TICKERS}?start={start}&limit={limit}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await httpClient.SendAsync(request);

                if (response != null && response.IsSuccessStatusCode)
                {
                    TickersResponse? result = null;
                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        result = await JsonSerializer.DeserializeAsync<TickersResponse>(contentStream);
                    }

                    if (result != null)
                    {
                        cryptoDetails = result.Data;
                    }
                }
            }

            return cryptoDetails;
        }
    }
}
