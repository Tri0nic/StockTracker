﻿using FluentScheduler;
using StockTracker.Models;

namespace StockTracker.Services
{
    public class NotificationService
    {
        private readonly IEnumerable<IMessageService> _messageServices;

        public NotificationService(IEnumerable<IMessageService> messageServices)
        {
            _messageServices = messageServices;
        }

        public void Notify(IEnumerable<Product> products, bool isEmailEnabled, bool isTelegramEnabled, int frequencyInMinutes)
        {
            //TODO: сделать цикл для каждого _messageServices отправлять сообщение?
            JobManager.AddJob(() => CreateLetter(products, isEmailEnabled, isTelegramEnabled),
            schedule => schedule.ToRunNow().AndEvery(frequencyInMinutes).Minutes());
        }

        public void CreateLetter(IEnumerable<Product> products, bool isEmailEnabled, bool isTelegramEnabled)
        {
            var letter = "";
            //TODO: переделать под одну отправку всех писем, а не по одному; Создать список/словарь
            //TODO: refactor - прокинуть isEmailEnabled и isTelegramEnabled через DI?
            foreach (var product in products.Where(p => p.IsTracked))
            {
                letter += $"\n {product}";
            }

            foreach (var service in _messageServices)
            {
                if (service is EmailService && isEmailEnabled)
                {
                    service.SendMessage($"{letter} is being tracked.");
                }

                if (service is TelegramService && isTelegramEnabled)
                {
                    service.SendMessage($"{letter} is being tracked.");
                }
            }
        }
    }
}