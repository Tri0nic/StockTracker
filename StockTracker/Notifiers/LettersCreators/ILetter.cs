using StockTracker.Models;

namespace StockTracker.Notifiers.LettersCreators
{
    public interface ILetter
    {
        string Create(IEnumerable<Product> availableProducts);
    }
}
