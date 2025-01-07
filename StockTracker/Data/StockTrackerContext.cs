using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockTracker.Models;

namespace StockTracker.Data
{
    public class StockTrackerContext : DbContext
    {
        public StockTrackerContext (DbContextOptions<StockTrackerContext> options)
            : base(options)
        {
        }

        public DbSet<StockTracker.Models.Product> Product { get; set; } = default!;
    }
}
