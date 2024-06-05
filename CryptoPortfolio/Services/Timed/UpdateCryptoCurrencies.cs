using CryptoPortfolio.Data;
using CryptoPortfolio.Models;
using CryptoPortfolio.Repositories.Interfaces;
using CryptoPortfolio.Services.Interfaces;

namespace CryptoPortfolio.Services.Timed
{
    public class UpdateCryptoCurrencies : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;

        private Timer? _timer;

        public UpdateCryptoCurrencies(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogging>();
                try
                {
                    var cryptoRepository = scope.ServiceProvider.GetService<ICryptoRepository>();

                    CryptoData.CurrenciesCount = await cryptoRepository.GetTotalCoinsCount();

                    var tasks = new List<Task<List<CryptoDetails>>>();
                    for (var i = 0; i < CryptoData.CurrenciesCount; i += 100)
                    {
                        var task = cryptoRepository.GetTickersData(i, 100);
                        tasks.Add(task);
                    }

                    await Task.WhenAll(tasks);

                    foreach (var task in tasks)
                    {
                        if (task != null && task.Result != null)
                        {
                            CryptoData.Currencies.AddRange(task.Result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.Log($"Error occured while getting CryptoData: {ex.Message}");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
