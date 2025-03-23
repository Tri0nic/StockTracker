using StockTracker.Models;

namespace StockTracker.Services.ParsersServices
{
    public interface IParser
    {
        string ShopName { get; }
        Task<string> Parse(Product product);
    }
}
