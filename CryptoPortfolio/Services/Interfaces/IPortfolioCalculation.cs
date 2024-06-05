using CryptoPortfolio.ViewModels;

namespace CryptoPortfolio.Services.Interfaces
{
    public interface IPortfolioCalculation
    {
        List<string> GetCurrenciesIds(IEnumerable<string> symbols);
        Task<HomeIndexViewModel> CalculatePortfolioValues(HomeIndexViewModel viewModel, IEnumerable<string> currenciesIds);
    }
}
