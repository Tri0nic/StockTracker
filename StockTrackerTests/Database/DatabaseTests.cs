using Microsoft.EntityFrameworkCore;
using Xunit;
using StockTracker.Data;

namespace StockTracker.Database.Tests
{
    [TestClass()]
    public class DatabaseTests
    {
        [TestMethod()]
        public void Database_ShouldHaveOnlyOneTrackedItemPerShop()
        {
            // Arrange
            // TODO: Убрать хардкод
            var options = new DbContextOptionsBuilder<StockTrackerContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=StockTracker.Data;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            using (var context = new StockTrackerContext(options))
            {
                // Act
                var shopsWithMultipleTrackedItems = context.Product
                    .GroupBy(s => s.Shop)
                    .Where(g => g.Count(s => s.IsTracked) > 1)
                    .Select(g => g.Key)
                    .ToList();

                // Assert
                Xunit.Assert.True(
                    shopsWithMultipleTrackedItems.Count == 0,
                    $"Найдены магазины с более чем одним отслеживаемым товаром: {string.Join(", ", shopsWithMultipleTrackedItems)}");
            }
        }
    }
}