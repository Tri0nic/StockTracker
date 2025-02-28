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
        private readonly TelegramSettings _telegramSettings;
        private readonly string token;
        private readonly TelegramBotClient botClient;
        public string ServiceName => "Telegram";
        public bool IsEnabled { get; set; }

        public TelegramNotifier(IOptions<TelegramSettings> telegramOptions)
        {
            _telegramSettings = telegramOptions.Value;
            token = _telegramSettings.Token;
            botClient = new TelegramBotClient(token);
        }

        public async Task SendMessage(string message)
        {
            var cts = new CancellationTokenSource();
            foreach (var recepient in _telegramSettings.Recipients)
            {
                await botClient.SendMessage(recepient, $"{message}");
            }

            Console.WriteLine("Bot is running...");

            cts.Cancel();
        }

        public string CreateLetter(IEnumerable<Product> availableProducts)
        {
            return TelegramLetter.Create(availableProducts);
        }
    }
}