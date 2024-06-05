using CryptoPortfolio.Models;

namespace CryptoPortfolio.Repositories.Interfaces
{
    public interface ICryptoRepository
    {
        Task<int> GetTotalCoinsCount();
        Task<List<CryptoDetails>> GetTickersData(int start, int limit);
        Task<Dictionary<string, decimal>> GetNewPrices(IEnumerable<string> currenciesIds);
    }
}
