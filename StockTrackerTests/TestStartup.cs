using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockTracker.Data;

namespace StockTrackerTests
{
    internal class TestStartup
    {
        public static IConfiguration Configuration { get; private set; }

        public static void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            services.AddDbContext<StockTrackerContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("StockTrackerContext")));
        }
    }
}
