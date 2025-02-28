using StockTracker.Models;

namespace StockTracker.Notifiers.LettersCreators
{
    public class EmailLetter
    {
        public static string Create(IEnumerable<Product> availableProducts)
        {
            var letter = "<table style='border-collapse: collapse; width: 85%;text-align: center; vertical-align: middle'>"
                         + "<tr><th style='border: 1px solid black; padding: 8px;text-align: center; vertical-align: middle'>Магазин</th>"
                         + "<th style='border: 1px solid black; padding: 8px;text-align: center; vertical-align: middle'>Товар</th>"
                         + "<th style='border: 1px solid black; padding: 8px;text-align: center; vertical-align: middle'>Количество</th></tr>";

            foreach (var product in availableProducts.Where(p => p.IsTracked))
            {
                letter += $"<tr><td style='border: 1px solid black; padding: 8px; width: 20%;text-align: center; vertical-align: middle'>{product.Shop}</td>"
                        + $"<td style='border: 1px solid black; padding: 8px; width: 60%;text-align: center; vertical-align: middle'><a href='{product.Link}'>{product.ProductName}</a></td>"
                        + $"<td style='border: 1px solid black; padding: 8px; width: 20%;text-align: center; vertical-align: middle'>{product.ProductCount}</td></tr>";
            }
            letter += "</table>";

            return letter;
        }
    }
}
