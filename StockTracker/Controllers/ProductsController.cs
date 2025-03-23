using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockTracker.Data;
using StockTracker.Models;
using StockTracker.Services.NotifiersServices;
using StockTracker.Services.ParsersServices;

namespace StockTracker.Controllers
{
    public class ProductsController : Controller
    {
        private static bool _isRunning = false;

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
            ViewBag.Services = _notificationService.GetAllServices();
            var uniqueProducts = await _context.Product
                .GroupBy(p => new { p.Shop, p.ProductName })
                .Select(g => g.OrderByDescending(p => p.ParseDate).First())
                .ToListAsync();

            return View(uniqueProducts);
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
        public async Task<IActionResult> Create([Bind("Id,Shop,ProductName,ProductCount,ParseDate,Link,IsTracked")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.ProductCount = "Не удалось спарсить";
                product.ParseDate = DateTime.Now;

                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["success"] = "Category created successfully";
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Shop,ProductName,ProductCount,ParseDate,Link,IsTracked")] Product product)
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
                    if (!_parserService.ProductExists(product.Id, _context))
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
            TempData["success"] = "Category edited successfully";

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
                return Problem("Entity set 'StockTrackerContext.Product' is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }
            await _context.SaveChangesAsync();

            TempData["success"] = "Category deleted successfully";

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStatistics(int id, string ProductName, string Shop)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'StockTrackerContext.Product' is null.");
            }

            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Statistics), new { ProductName, Shop });
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

        [HttpPost]
        public IActionResult StopProcess()
        {
            Console.WriteLine("Stopping the application...");
            Environment.Exit(0);
            return Ok();
        }

        [HttpPost]
        //TODO: Изменить название?
        public async Task<IActionResult> SendNotifications(Dictionary<string, bool> servicePreferences, int frequencyInMinutes)
        {
            foreach (var entry in servicePreferences)
            {
                _notificationService.SetServiceStatus(entry.Key, entry.Value);
            }

            var products = await _context.Product.Where(p => p.IsTracked).ToListAsync();
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
                        await _parserService.ParseAndNotify(products, _context, _notificationService);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка Ошибка !!!: {ex.Message}");
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

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Products/Statistics
        public async Task<IActionResult> Statistics(string ProductName, string Shop)
        {
            var products = await _context.Product
                .Where(product => product.ProductName == ProductName && product.Shop == Shop)
                .OrderBy(product => product.ParseDate)
                .ToListAsync();

            // Подготовка данных для графика
            ViewBag.ChartLabels = products
                .Select(p => p.ParseDate.HasValue ? p.ParseDate.Value.ToString("yyyy-MM-dd HH:mm") : "")
                .ToList();
            ViewBag.ChartData = products.Select(p => p.ProductCount).ToList();

            return View(products);
        }
    }
}
