using StockTracker.Models;

namespace StockTracker.Notifiers.LettersCreators
{
    public class TelegramLetter : ILetter
    {
        public string Create(IEnumerable<Product> availableProducts)
        {
            var letter = "Магазин | Товар | Количество\n";

            foreach (var product in availableProducts.Where(p => p.IsTracked))
            {
                letter += $"{product.Shop} | {product.ProductName} | {product.ProductCount}\n";
            }
            //{product.Link}
            return letter;
        }
    }
}
