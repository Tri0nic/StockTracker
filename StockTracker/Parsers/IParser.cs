using StockTracker.Models;

namespace StockTracker.Parsers
{
    public interface IParser
    {
        Task<bool> Parse(string url);
    }
}
