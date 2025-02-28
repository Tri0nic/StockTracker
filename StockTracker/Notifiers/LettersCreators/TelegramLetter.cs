using StockTracker.Models;

namespace StockTracker.Notifiers.LettersCreators
{
    public class TelegramLetter
    {
        public static string Create(IEnumerable<Product> availableProducts)
        {
            var letter = "Магазин | Товар | Количество\n";

            foreach (var product in availableProducts.Where(p => p.IsTracked))
            {
                letter += $"{product.Shop} | {product.ProductName} | {product.ProductCount}\n";
            }
            
            return letter;
        }
    }
}
