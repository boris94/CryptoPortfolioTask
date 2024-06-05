namespace CryptoPortfolio.ViewModels
{
    public class HomeIndexViewModel
    {
        public IFormFile? UploadedPortfolio { get; set; }
        public List<CurrencyDetails> Currencies { get; set; } = [];
        public decimal? InitialPortfolioValue { get; set; }
        public decimal? CurrentPortfolioValue { get; set; }
        public string? PortfolioValueChange { get; set; }

        public string UploadFileErrorMessage { get; set; }
    }
}
