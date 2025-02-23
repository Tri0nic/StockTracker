using System.Net.Mail;
using System.Net;
using StockTracker.Configurations;
using Microsoft.Extensions.Options;
using StockTracker.Services.NotifiersServices;
using StockTracker.Notifiers.LettersCreators;
namespace StockTracker.Notifiers
{
    public class EmailNotifier : INotifierService
    {
        private readonly EmailSettings _emailSettings;

        public string ServiceName => "Email";
        public bool IsEnabled { get; set; }
        public ILetter Letter { get; }

        public EmailNotifier(IOptions<EmailSettings> emailSettings, ILetter letter)
        {
            _emailSettings = emailSettings.Value;
            IsEnabled = false;
            Letter = letter;
        }

        public async Task SendMessage(string message)
        {
            using (SmtpClient smtpClient = new SmtpClient(_emailSettings.Server, _emailSettings.Port))
            {
                SettingSmtpClient(smtpClient);
                using (MailMessage mailMessage = new MailMessage())
                {
                    SettingMailMessage(mailMessage, message);

                    AddRecipients(_emailSettings, mailMessage);

                    try
                    {
                        smtpClient.Send(mailMessage);
                        Console.WriteLine(message);
                        Console.WriteLine("Уведомление отправлено на email");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка отправки сообщения: {ex.Message}");
                    }
                }
            }
        }

        private void SettingSmtpClient(SmtpClient smtpClient)
        {
            smtpClient.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
            smtpClient.EnableSsl = true;
        }

        private void SettingMailMessage(MailMessage mailMessage, string message)
        {
            mailMessage.From = new MailAddress(_emailSettings.Username);
            mailMessage.Subject = "Товар доступен на сайте";

            mailMessage.IsBodyHtml = true;
            mailMessage.Body = message;
        }

        private void AddRecipients(EmailSettings _emailSettings, MailMessage mailMessage)
        {
            foreach (var recipient in _emailSettings.Recipients)
            {
                mailMessage.To.Add(recipient);
            }
        }
    }
}
