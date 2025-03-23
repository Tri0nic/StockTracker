using Microsoft.EntityFrameworkCore;
using Serilog;
using StockTracker.Configurations;
using StockTracker.Data;
using StockTracker.Notifiers;
using StockTracker.Parsers;
using StockTracker.Services.NotifiersServices;
using StockTracker.Services.ParsersServices;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("Logs/all/log-all.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le =>
            le.Properties.ContainsKey("SourceContext") &&
            le.Properties["SourceContext"].ToString().Contains("StockTracker"))
        .WriteTo.File("Logs/mine/log-mine.txt", rollingInterval: RollingInterval.Day))
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddDbContext<StockTrackerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StockTrackerContext") ?? throw new InvalidOperationException("Connection string 'StockTrackerContext' not found.")));
builder.Services.AddSingleton(builder.Configuration["DriverDirectory"]);

// Add services to the container.
builder.Services.AddTransient<ParserService>();
builder.Services.AddTransient<NotificationService>();
builder.Services.AddTransient<ProxyService>();
builder.Services.AddTransient<INotifierService, EmailNotifier>();
builder.Services.AddTransient<INotifierService, TelegramNotifier>();

builder.Services.AddTransient<IParser, MosigraParser>();
builder.Services.AddTransient<IParser, YandexMarketParser>();
builder.Services.AddTransient<IParser, HobbyGamesParser>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<TelegramSettings>(builder.Configuration.GetSection("TelegramSettings"));


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.Run();
