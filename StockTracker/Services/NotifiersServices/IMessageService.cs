namespace StockTracker.Services.NotifiersServices
{
    public interface IMessageService
    {
        string ServiceName { get; }
        bool IsEnabled { get; set; }
        void SendMessage(string message);
    }
}
