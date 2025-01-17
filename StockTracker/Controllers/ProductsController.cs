﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FluentScheduler;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockTracker.Data;
using StockTracker.Models;
using StockTracker.Parsers;
using StockTracker.Services;

namespace StockTracker.Controllers
{
    public class ProductsController : Controller
    {
        private readonly StockTrackerContext _context;
        private readonly NotificationService _notificationService;
        private readonly ParserService _parserService;

        public ProductsController(StockTrackerContext context, NotificationService notificationService, ParserService parserService)
        {
            _context = context;
            _notificationService = notificationService;
            _parserService = parserService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
              return _context.Product != null ? 
                          View(await _context.Product.ToListAsync()) :
                          Problem("Entity set 'StockTrackerContext.Product'  is null.");
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Shop,ProductName,Link,IsTracked")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Shop,ProductName,Link,IsTracked")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'StockTrackerContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> UpdateIsTracked(int id, bool isTracked)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.IsTracked = isTracked;
            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private static bool _isRunning = false;
        //Сделать метод Track, который будет ссылаться на парсер? Вместо SendNotifications.
        //А SendNotifications вызывать уже в методе Track если парсер вернул true??
        [HttpPost]
        //TODO: Изменить название?
        //TODO: Передавать коллекцию сервисов вместо явных isEmailEnabled isTelegramEnabled IEnumerable<IMessageService> notificationServices
        //, bool isEmailEnabled, bool isTelegramEnabled
        //IEnumerable<bool> notifications
        public async Task<IActionResult> SendNotifications(bool isEmailEnabled, bool isTelegramEnabled, int frequencyInMinutes)
        {
            var products = await _context.Product.Where(p => p.IsTracked).ToListAsync();

            //TODO: сделать цикл для каждого _messageServices отправлять сообщение?
            //JobManager.AddJob(() => ParseAndNotify(products, isEmailEnabled, isTelegramEnabled),
            //schedule => schedule.ToRunNow().AndEvery(frequencyInMinutes).Minutes());

            //TODO: убрать этот цикл в отдельный класс
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    var cycleStartTime = DateTime.Now;

                    if (_isRunning)
                    {
                        Console.WriteLine("Previous task is still running. Skipping this cycle.");
                    }
                    else
                    {
                        try
                        {
                            _isRunning = true;
                            await ParseAndNotify(products, isEmailEnabled, isTelegramEnabled);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        finally
                        {
                            _isRunning = false;
                        }
                    }

                    var elapsedTime = DateTime.Now - cycleStartTime;
                    var delayTime = TimeSpan.FromMinutes(frequencyInMinutes) - elapsedTime;

                    if (delayTime > TimeSpan.Zero)
                    {
                        await Task.Delay(delayTime);
                    }
                }
            });


            TempData.Keep(); // Сохраняем настройки

            return RedirectToAction(nameof(Index)); // Возвращаемся на главную страницу
        }

        public async Task ParseAndNotify(IEnumerable<Product> products, bool isEmailEnabled, bool isTelegramEnabled)
        {
            var productIsAvailable = await _parserService.ParseProducts(products);

            if (productIsAvailable)
            {
                _notificationService.Notify(products, isEmailEnabled, isTelegramEnabled);
            }
        }

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
