using StockTracker.Models;

namespace StockTracker.Services.ParsersServices
{
    public class ParserService
    {
        private readonly Dictionary<string, IParser> _parsers;
        private readonly ILogger<ParserService> _logger;

        public ParserService(IEnumerable<IParser> parsers, ILogger<ParserService> logger)
        {
            _parsers = parsers.ToDictionary(parsers => parsers.ShopName);
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> ParseProducts(IEnumerable<Product> products)
        {
            var availableProducts = new List<Product>();

            _logger.LogInformation("Парсинг продуктов...");
            foreach (var product in products)
            {
                _logger.LogInformation($"Начат парсинг товара: {product.Shop} --- {product.ProductName}");

                var parserResult = ParseProduct(product).Result;

                product.IsTracked = false;

                availableProducts.Add(CreateParsedProduct(product, parserResult));
            }
            _logger.LogInformation("Все продукты были спаршены...");

            return availableProducts;
        }

        public async Task<string> ParseProduct(Product product)
        {
            if (!_parsers.TryGetValue(product.Shop, out var parser))
            {
                _logger.LogError($"Парсинг для магазина {product.Shop} не реализован.");
                throw new NotImplementedException($"Парсинг для магазина {product.Shop} не реализован.");
            }

            return await parser.Parse(product);
        }

        private Product CreateParsedProduct(Product product, string parserResult)
        {
            var newProduct = new Product 
            { 
            Id = 0,
            Shop = product.Shop,
            ProductName = product.ProductName,
            ParseDate = DateTime.Now,
            ProductCount = parserResult,
            Link = product.Link,
            IsTracked = true
            };

            return newProduct;
        }
    }
}
