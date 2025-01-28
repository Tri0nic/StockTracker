using System.Net.Mail;
using System.Net;
using StockTracker.Configurations;
using Microsoft.Extensions.Options;
using StockTracker.Services.NotifiersServices;

namespace StockTracker.Notifiers
{
    public class EmailNotifier : IMessageService
    {
        private readonly EmailSettings _emailSettings;

        public EmailNotifier(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public void SendMessage(string message)
        {
            using (SmtpClient smtpClient = new SmtpClient(_emailSettings.Server, _emailSettings.Port))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                smtpClient.EnableSsl = true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_emailSettings.Username);
                    mailMessage.Subject = "Товар доступен на сайте";

                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = message;

                    // Добавляем всех адресатов из массива
                    foreach (var recipient in _emailSettings.Recipients)
                    {
                        mailMessage.To.Add(recipient);
                    }

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
    }
}
