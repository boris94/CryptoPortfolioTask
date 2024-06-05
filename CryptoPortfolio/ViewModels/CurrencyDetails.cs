namespace CryptoPortfolio.ViewModels
{
    public class CurrencyDetails
    {
        public string Name { get; set; }
        public decimal Ammount { get; set; }
        public decimal InitialPrice { get; set; }

        public decimal? CurrentPrice { get; set; }
        public string PriceChange { get; set; }
    }
}
