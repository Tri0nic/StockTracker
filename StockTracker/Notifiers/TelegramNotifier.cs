using StockTracker.Services.NotifiersServices;
using Telegram.Bot;
using StockTracker.Notifiers.LettersCreators;

namespace StockTracker.Notifiers
{
    public class TelegramNotifier : INotifierService
    {
        public string ServiceName => "Telegram";
        public bool IsEnabled { get; set; }
        public ILetter Letter { get; }

        private static readonly string token = "";
        private static readonly TelegramBotClient botClient = new TelegramBotClient(token);

        public TelegramNotifier(ILetter letter) 
        { 
            Letter = letter;
        }

        public async Task SendMessage(string message)
        {
            var cts = new CancellationTokenSource();
            await botClient.SendMessage(, $"{Letter}!");

            Console.WriteLine("Bot is running...");
            Console.ReadLine();

            cts.Cancel();
        }
    }
}