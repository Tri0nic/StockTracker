using StockTracker.Models;

namespace StockTracker.Parsers
{
    public interface IParser
    {
        Task<string> Parse(string url);
    }
}
