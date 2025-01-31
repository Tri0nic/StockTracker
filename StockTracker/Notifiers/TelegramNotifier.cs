using StockTracker.Services.NotifiersServices;

namespace StockTracker.Notifiers
{
    public class TelegramNotifier : IMessageService
    {
        public string ServiceName => "Telegram";
        public bool IsEnabled { get; set; }

        public void SendMessage(string message)
        {
            Console.WriteLine($"Telegram: {message}");
        }
    }
}