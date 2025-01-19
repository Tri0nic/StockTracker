using StockTracker.Models;

namespace StockTracker.Parsers
{
    public interface IParser
    {
        Task<(bool, int)> Parse(string url);
    }
}
