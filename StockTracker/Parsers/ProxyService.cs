using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace StockTracker.Parsers
{
    public class ProxyService
    {
        private readonly List<string> _proxies;

        public ProxyService(IConfiguration configuration)
        {
            _proxies = configuration.GetSection("Proxies").Get<List<string>>() ?? new List<string>();
        }

        public ChromeOptions GetRandomProxy()
        {
            if (_proxies == null || _proxies.Count == 0)
            {
                throw new InvalidOperationException("No proxies available in the configuration.");
            }

            Random rnd = new Random();
            var randomProxy = _proxies[rnd.Next(_proxies.Count)];

            var proxy = new Proxy
            {
                HttpProxy = randomProxy
            };

            var options = new ChromeOptions();
            options.Proxy = proxy;

            return options;
        }
    }
}
