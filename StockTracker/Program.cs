using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockTracker.Data;
using StockTracker.Parsers;
using StockTracker.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<StockTrackerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StockTrackerContext") ?? throw new InvalidOperationException("Connection string 'StockTrackerContext' not found.")));

// Add services to the container.
builder.Services.AddTransient<ParserService>();
builder.Services.AddTransient<NotificationService>();
builder.Services.AddTransient<IMessageService, EmailService>();
builder.Services.AddTransient<IMessageService, TelegramService>();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
