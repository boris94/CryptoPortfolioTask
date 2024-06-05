using CryptoPortfolio.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace CryptoPortfolio.Services
{
    public class LoggingService : ILogging
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LoggingService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void Log(string message)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "Log.txt");

            var append = File.Exists(path);

            using (var outputFile = new StreamWriter(path, append))
            {
                outputFile.WriteLine(message);
            }
        }
    }
}
