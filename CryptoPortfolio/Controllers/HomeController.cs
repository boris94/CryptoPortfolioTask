using CryptoPortfolio.Services.Interfaces;
using CryptoPortfolio.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CryptoPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPortfolioCalculation _portfolioCalculation;
        private ILogging _logger;

        private const string VIEWMODEL_NAME = "HomeIndexViewModel";

        public HomeController(IPortfolioCalculation portfolioCalculation, ILogging logger)
        {
            _portfolioCalculation = portfolioCalculation;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeIndexViewModel()
            {
                Currencies = new List<CurrencyDetails>(64)
            };

            var serializedViewModel = HttpContext.Session.GetString(VIEWMODEL_NAME);
            if (!string.IsNullOrEmpty(serializedViewModel))
            {
                viewModel = JsonSerializer.Deserialize<HomeIndexViewModel>(serializedViewModel);

                var currenciesIds = _portfolioCalculation.GetCurrenciesIds(viewModel.Currencies.Select(x => x.Name));

                viewModel = await _portfolioCalculation.CalculatePortfolioValues(viewModel, currenciesIds);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(HomeIndexViewModel inputData)
        {
            var viewModel = new HomeIndexViewModel
            {
                Currencies = new List<CurrencyDetails>(64)
            };

            if (inputData == null || inputData.UploadedPortfolio == null)
            {
                return View(viewModel);
            }

            try
            {
                using (var sr = new StreamReader(inputData.UploadedPortfolio.OpenReadStream()))
                {
                    //TODO: If there is time put file format validation
                    var line = sr.ReadLine();
                    while (line != null)
                    {
                        string[] currencyData = line.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        var currencyDetails = new CurrencyDetails
                        {
                            Ammount = decimal.Parse(currencyData[0]),
                            Name = currencyData[1],
                            InitialPrice = decimal.Parse(currencyData[2])
                        };

                        viewModel.Currencies.Add(currencyDetails);
                        line = sr.ReadLine();
                    }

                    _logger.Log($"Uploaded portfolio {inputData.UploadedPortfolio.FileName}");
                }
            }
            catch (Exception ex)
            {
                var error = $"Error occured while uploading file {inputData.UploadedPortfolio.FileName}";
                _logger.Log($"{error}: {ex.Message}");
                viewModel.UploadFileErrorMessage = error;

                return View(viewModel);
            }

            var serializedViewModel = JsonSerializer.Serialize(viewModel);
            HttpContext.Session.SetString(VIEWMODEL_NAME, serializedViewModel);

            var currenciesIds = _portfolioCalculation.GetCurrenciesIds(viewModel.Currencies.Select(x => x.Name));

            viewModel = await _portfolioCalculation.CalculatePortfolioValues(viewModel, currenciesIds);

            return View(viewModel);
        }
    }
}
