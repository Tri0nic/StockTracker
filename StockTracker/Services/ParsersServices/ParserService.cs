using StockTracker.Data;
using StockTracker.Models;
using StockTracker.Services.NotifiersServices;

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

        private async Task<IEnumerable<Product>> ParseProducts(IEnumerable<Product> products)
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

        public bool ProductExists(int id, StockTrackerContext context)
        {
            return (context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task ParseAndNotify(IEnumerable<Product> products, StockTrackerContext context, NotificationService notificationService)
        {
            var allProducts = await ParseProducts(products);

            AddProductsToContext(allProducts, context);

            await context.SaveChangesAsync();
            _logger.LogInformation("Успешное сохранение в БД");

            var availableProducts = allProducts.Where(p => int.TryParse(p.ProductCount, out _)).ToList();

            if (availableProducts.Any())
            {
                _logger.LogInformation("Запуск рассылки уведомлений...");
                notificationService.Notify(availableProducts);
            }
        }

        private void AddProductsToContext(IEnumerable<Product> allProducts, StockTrackerContext context)
        {
            foreach (var product in allProducts)
            {
                context.Add(product);
            }
        }
    }
}
