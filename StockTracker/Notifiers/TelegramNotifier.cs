using StockTracker.Services.NotifiersServices;
using Telegram.Bot;
using StockTracker.Notifiers.LettersCreators;
using StockTracker.Models;
using StockTracker.Configurations;
using Microsoft.Extensions.Options;

namespace StockTracker.Notifiers
{
    public class TelegramNotifier : INotifierService
    {
        private readonly string token;
        private readonly TelegramBotClient botClient;
        private readonly TelegramSettings _telegramSettings;
        private readonly ILogger<TelegramNotifier> _logger;
       
        public string ServiceName => "Telegram";
        public bool IsEnabled { get; set; }

        public TelegramNotifier(IOptions<TelegramSettings> telegramOptions, ILogger<TelegramNotifier> logger)
        {
            _telegramSettings = telegramOptions.Value;
            token = _telegramSettings.Token;
            botClient = new TelegramBotClient(token);
            _logger = logger;
        }

        public async Task SendMessage(string message)
        {
            _logger.LogInformation($"Отправка сообщения в telegram...");
            try
            {
                Send(message);
                _logger.LogInformation("Сообщение отправлено в telegram");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка отправки сообщения: {ex.Message}");
            }
        }

        public async void Send(string message)
        {
            var cts = new CancellationTokenSource();
            foreach (var recepient in _telegramSettings.Recipients)
            {
                await botClient.SendMessage(recepient, $"{message}");
            }

            cts.Cancel();
        }

        public string CreateLetter(IEnumerable<Product> availableProducts)
        {
            return TelegramLetter.Create(availableProducts, _logger);
        }
    }
}