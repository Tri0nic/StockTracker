using StockTracker.Models;

namespace StockTracker.Parsers
{
    public interface IParser
    {
        bool Parse(Product products);
    }
}
