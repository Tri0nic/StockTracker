namespace StockTracker.Services
{
    public class EmailService : IMessageService
    {
        public void SendMessage(string message)
        {
            Console.WriteLine($"Email: {message}");
        }
    }
}