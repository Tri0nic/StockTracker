using StockTracker.Data;
using Microsoft.Extensions.DependencyInjection;
using StockTrackerTests;

namespace StockTracker.Database.Tests
{
    [TestClass()]
    public class DatabaseTests
    {
        private readonly StockTrackerContext _context;

        public DatabaseTests()
        {
            var services = new ServiceCollection();
            TestStartup.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            _context = serviceProvider.GetRequiredService<StockTrackerContext>();
        }

        [TestMethod()]
        public void Database_ShouldHaveOnlyOneTrackedItemPerShop()
        {
            // Act
            var shopsWithMultipleTrackedItems = _context.Product
                .GroupBy(s => new { s.Shop, s.ProductName })
                .Where(g => g.Count(s => s.IsTracked) > 1)
                .Select(g => g.Key)
                .ToList();

            // Assert
            Xunit.Assert.True(
                shopsWithMultipleTrackedItems.Count == 0,
                shopsWithMultipleTrackedItems.Any()
                ? $"Найдены магазины с более чем одним одинаковым отслеживаемым товаром: {string.Join(" --- ", shopsWithMultipleTrackedItems[0].Shop, shopsWithMultipleTrackedItems[0].ProductName)}"
                : "Нет магазинов с более чем одним одинаковым отслеживаемым товаром.");
        }
    }
}