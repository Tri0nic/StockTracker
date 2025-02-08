using StockTracker.Models;

namespace StockTracker.Services.ParsersServices
{
    public class ParserService
    {
        private readonly Dictionary<string, IParser> _parsers;

        public ParserService(IEnumerable<IParser> parsers)
        {
            _parsers = parsers.ToDictionary(parsers => parsers.ShopName);
        }

        public async Task<IEnumerable<Product>> ParseProducts(IEnumerable<Product> products)
        {
            var availableProducts = new List<Product>();

            foreach (var product in products)
            {
                await Console.Out.WriteLineAsync($"\nНачали парсинг! {product.ProductName}\n");

                var parserResult = ParseProduct(product).Result;

                product.IsTracked = false;

                availableProducts.Add(CreateParsedProduct(product, parserResult));
            }
            return availableProducts;
        }

        public async Task<string> ParseProduct(Product product)
        {
            if (_parsers.TryGetValue(product.Shop, out var parser))
                return await parser.Parse(product.Link);
            else
                throw new NotImplementedException($"Парсинг для магазина {product.Shop} не реализован.");
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
