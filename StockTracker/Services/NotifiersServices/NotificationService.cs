using StockTracker.Models;

namespace StockTracker.Services.NotifiersServices
{
    public class NotificationService
    {
        private readonly IEnumerable<IMessageService> _messageServices;

        public NotificationService(IEnumerable<IMessageService> messageServices)
        {
            _messageServices = messageServices;
        }

        public IEnumerable<IMessageService> GetAllServices()
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
            var letter = CreateLetter(availableProducts);

            foreach (var service in _messageServices.Where(s => s.IsEnabled))
            {
                service.SendMessage(letter);
            }
        }

        private string CreateLetter(IEnumerable<Product> availableProducts)
        {

            var letter = "<table style='border-collapse: collapse; width: 85%;text-align: center; vertical-align: middle'>"
                         + "<tr><th style='border: 1px solid black; padding: 8px;text-align: center; vertical-align: middle'>Магазин</th>"
                         + "<th style='border: 1px solid black; padding: 8px;text-align: center; vertical-align: middle'>Товар</th>"
                         + "<th style='border: 1px solid black; padding: 8px;text-align: center; vertical-align: middle'>Количество</th>"
                         + "<th style='border: 1px solid black; padding: 8px;text-align: center; vertical-align: middle'>Ссылка</th></tr>";

            foreach (var product in availableProducts.Where(p => p.IsTracked))
            {
                letter += $"<tr><td style='border: 1px solid black; padding: 8px; width: 20%;text-align: center; vertical-align: middle'>{product.Shop}</td>"
                        + $"<td style='border: 1px solid black; padding: 8px; width: 60%;text-align: center; vertical-align: middle'>{product.ProductName}</td>"
                        + $"<td style='border: 1px solid black; padding: 8px; width: 20%;text-align: center; vertical-align: middle'>{product.ProductCount}</td>"
                        + $"<td style='border: 1px solid black; padding: 8px; width: 20%;text-align: center; vertical-align: middle'><a href='{product.Link}'>Ссылка</a></td></tr>";
            }
            letter += "</table>";

            return letter;
        }
    }
}