using System.Net.Mail;
using System.Net;
using StockTracker.Configurations;
using Microsoft.Extensions.Options;
using StockTracker.Services.NotifiersServices;
using StockTracker.Notifiers.LettersCreators;
using StockTracker.Models;
namespace StockTracker.Notifiers
{
    public class EmailNotifier : INotifierService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailNotifier> _logger;

        public string ServiceName => "Email";
        public bool IsEnabled { get; set; }

        public EmailNotifier(IOptions<EmailSettings> emailSettings, ILogger<EmailNotifier> logger)
        {
            _emailSettings = emailSettings.Value;
            IsEnabled = false;
            _logger = logger;
        }

        public async Task SendMessage(string message)
        {
            _logger.LogInformation($"Формирование письма и отправка его на email...");
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
                        _logger.LogInformation("Письмо отправлено на email");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Ошибка отправки сообщения: {ex.Message}");
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
            mailMessage.Subject = "Товары доступны на сайте";

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

        public string CreateLetter(IEnumerable<Product> availableProducts)
        {
            return EmailLetter.Create(availableProducts, _logger);
        }
    }
}
