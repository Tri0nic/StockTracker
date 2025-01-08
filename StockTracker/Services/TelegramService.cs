namespace StockTracker.Services
{
    public class TelegramService : IMessageService
    {
        public void SendMessage(string message)
        {
            Console.WriteLine($"Telegram: {message}");
        }
    }
}