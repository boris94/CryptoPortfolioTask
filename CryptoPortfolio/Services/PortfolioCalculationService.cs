using CryptoPortfolio.Data;
using CryptoPortfolio.Repositories.Interfaces;
using CryptoPortfolio.Services.Interfaces;
using CryptoPortfolio.ViewModels;

namespace CryptoPortfolio.Services
{
    public class PortfolioCalculationService : IPortfolioCalculation
    {
        private readonly ICryptoRepository _cryptoRepository;

        public PortfolioCalculationService(ICryptoRepository cryptoRepository)
        {
            _cryptoRepository = cryptoRepository;
        }

        public List<string> GetCurrenciesIds(IEnumerable<string> symbols)
        {
            var ids = new List<string>();
            foreach (var symbol in symbols)
            {
                var cryptoDetails = CryptoData.Currencies.SingleOrDefault(x => x.Symbol == symbol);

                if (cryptoDetails != null)
                {
                    ids.Add(cryptoDetails.Id);
                }
            }

            return ids;
        }

        public async Task<HomeIndexViewModel> CalculatePortfolioValues(HomeIndexViewModel viewModel, IEnumerable<string> currenciesIds)
        {
            var newPrices = await _cryptoRepository.GetNewPrices(currenciesIds);

            foreach (var currency in viewModel.Currencies)
            {
                if (newPrices.TryGetValue(currency.Name, out decimal newPrice))
                {
                    currency.PriceChange = CalculatePercentageDifference(currency.InitialPrice, newPrice);
                    currency.CurrentPrice = newPrice;
                }
            }

            viewModel.InitialPortfolioValue = Math.Round(viewModel.Currencies.Sum(x => x.InitialPrice * x.Ammount), 2);
            //If a coin could not be found in the API result there will be no price change so use the initial price
            viewModel.CurrentPortfolioValue = Math.Round(
                viewModel.Currencies.Where(x => !string.IsNullOrEmpty(x.PriceChange)).Sum(x => x.CurrentPrice.Value * x.Ammount)
                + viewModel.Currencies.Where(x => string.IsNullOrEmpty(x.PriceChange)).Sum(x => x.InitialPrice * x.Ammount), 2);
            viewModel.PortfolioValueChange = CalculatePercentageDifference(viewModel.InitialPortfolioValue.Value, viewModel.CurrentPortfolioValue.Value);

            return viewModel;
        }

        private string CalculatePercentageDifference(decimal initailValue, decimal currentValue)
        {
            var priceDifference = Math.Abs(currentValue - initailValue);
            var priceDifferencePercentage = Math.Round(priceDifference / initailValue * 100, 1);
            var sign = currentValue < initailValue ? "-" : "+";

            return $"{sign}{priceDifferencePercentage}%";
        }
    }
}
