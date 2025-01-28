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
                UpdateProductDetails(product, parserResult);

                availableProducts.Add(product);
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

        private void UpdateProductDetails(Product product, string parserResult)
        {
            product.ProductCount = parserResult;
            product.ParseDate = DateTime.Now;
        }
    }
}
