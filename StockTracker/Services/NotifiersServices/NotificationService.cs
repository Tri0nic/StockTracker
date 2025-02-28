using StockTracker.Models;

namespace StockTracker.Services.NotifiersServices
{
    public class NotificationService
    {
        private readonly IEnumerable<INotifierService> _messageServices;

        public NotificationService(IEnumerable<INotifierService> messageServices)
        {
            _messageServices = messageServices;
        }

        public IEnumerable<INotifierService> GetAllServices()
        {
            return _messageServices;
        }

        public void SetServiceStatus(string serviceName, bool isEnabled)
        {
            var service = _messageServices.FirstOrDefault(s => s.ServiceName == serviceName);
            if (service != null)
            {
                service.IsEnabled = isEnabled;
            }
        }

        public void Notify(IEnumerable<Product> availableProducts)
        {
            foreach (var service in _messageServices.Where(s => s.IsEnabled))
            {
                var letter = service.CreateLetter(availableProducts);
                service.SendMessage(letter);
            }
        }
    }
}