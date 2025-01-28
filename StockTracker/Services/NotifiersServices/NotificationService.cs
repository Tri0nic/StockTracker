using StockTracker.Models;
using StockTracker.Notifiers;

//TODO: переделать под одну отправку всех писем, а не по одному; Создать список/словарь
//TODO: refactor - прокинуть isEmailEnabled и isTelegramEnabled через DI?

namespace StockTracker.Services.NotifiersServices
{
    public class NotificationService
    {
        private readonly IEnumerable<IMessageService> _messageServices;

        public NotificationService(IEnumerable<IMessageService> messageServices)
        {
            _messageServices = messageServices;
        }

        public void Notify(IEnumerable<Product> availableProducts, bool isEmailEnabled, bool isTelegramEnabled)
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

            foreach (var service in _messageServices)
            {
                if (service is EmailNotifier && isEmailEnabled)
                {
                    service.SendMessage($"{letter}");
                }

                if (service is TelegramNotifier && isTelegramEnabled)
                {
                    //service.SendMessage($"The following products are being tracked:\n\n{letter}");
                }
            }
        }
    }
}