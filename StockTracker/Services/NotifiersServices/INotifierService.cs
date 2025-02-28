using StockTracker.Models;

namespace StockTracker.Services.NotifiersServices
{
    public interface INotifierService
    {
        string ServiceName { get; }
        bool IsEnabled { get; set; }
        Task SendMessage(string message);
        string CreateLetter(IEnumerable<Product> availableProducts);
    }
}