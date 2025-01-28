using StockTracker.Services.NotifiersServices;

namespace StockTracker.Notifiers
{
    public class TelegramNotifier : IMessageService
    {
        public void SendMessage(string message)
        {
            Console.WriteLine($"Telegram: {message}");
        }
    }
}