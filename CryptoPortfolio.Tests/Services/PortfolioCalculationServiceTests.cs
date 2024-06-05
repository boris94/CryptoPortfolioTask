using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using CryptoPortfolio.Repositories.Interfaces;
using CryptoPortfolio.Data;
using CryptoPortfolio.Models;
using CryptoPortfolio.ViewModels;

namespace CryptoPortfolio.Services.Tests
{
    [TestClass()]
    public class PortfolioCalculationServiceTests
    {
        public PortfolioCalculationServiceTests()
        {
        }

        [TestMethod()]
        public void GetCurrenciesIds_ReturnsCorrectData()
        {
            //Arrange
            var cryptoRepository = Substitute.For<ICryptoRepository>();
            var portfolioCalculation = new PortfolioCalculationService(cryptoRepository);

            CryptoData.Currencies = new List<CryptoDetails>
            {
                new CryptoDetails
                {
                    Id = "4",
                    Symbol = "BTC"
                },
                new CryptoDetails
                {
                    Id = "1",
                    Symbol = "SHIB"
                },
                new CryptoDetails
                {
                    Id = "2",
                    Symbol = "ETH"
                }
            };

            var symbols = new List<string> { "ETH", "BTC" };

            var expectedResult = new List<string> { "2", "4" };

            //Act
            var result = portfolioCalculation.GetCurrenciesIds(symbols);

            //Assert
            CollectionAssert.AreEquivalent(result, expectedResult);
        }

        [TestMethod()]
        public async Task CalculatePortfolioValues_ReturnsCorrectValues()
        {
            //Arrange
            var cryptoRepository = Substitute.For<ICryptoRepository>();
            var portfolioCalculation = new PortfolioCalculationService(cryptoRepository);

            var newPrices = new Dictionary<string, decimal>
            {
                { "ETH", 3100 },
                { "BTC", 75000 }
            };
            cryptoRepository.GetNewPrices(Arg.Any<IEnumerable<string>>()).Returns(newPrices);

            var viewModel = new HomeIndexViewModel
            {
                Currencies = new List<CurrencyDetails>
                {
                    new CurrencyDetails
                    {
                        Name = "ETH",
                        InitialPrice = 1500,
                        Ammount = 11
                    },
                    new CurrencyDetails
                    {
                        Name = "BTC",
                        InitialPrice = 50000,
                        Ammount = 3
                    },
                    new CurrencyDetails
                    {
                        Name = "SHIB",
                        InitialPrice = 25,
                        Ammount = 200
                    }
                }
            };

            //Act
            var result = await portfolioCalculation.CalculatePortfolioValues(viewModel, new List<string>());

            //Assert
            var currency = result.Currencies.Where(x => x.Name == "ETH").SingleOrDefault();
            Assert.IsNotNull(currency);
            Assert.AreEqual(newPrices[currency.Name], currency.CurrentPrice);
            Assert.AreEqual("+106.7%", currency.PriceChange);

            currency = result.Currencies.Where(x => x.Name == "BTC").SingleOrDefault();
            Assert.IsNotNull(currency);
            Assert.AreEqual(newPrices[currency.Name], currency.CurrentPrice);
            Assert.AreEqual("+50.0%", currency.PriceChange);

            currency = result.Currencies.Where(x => x.Name == "SHIB").SingleOrDefault();
            Assert.IsNotNull(currency);
            Assert.IsNull(currency.CurrentPrice);
            Assert.IsNull(currency.PriceChange);

            Assert.AreEqual(171500, result.InitialPortfolioValue);
            Assert.AreEqual(264100, result.CurrentPortfolioValue);
            Assert.AreEqual("+54.0%", result.PortfolioValueChange);
        }
    }
}