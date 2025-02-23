using StockTracker.Models;
using StockTracker.Notifiers.LettersCreators;

namespace StockTracker.Services.NotifiersServices
{
    public interface INotifierService
    {
        string ServiceName { get; }
        bool IsEnabled { get; set; }
        Task SendMessage(string message);
        ILetter Letter { get; }
    }
}