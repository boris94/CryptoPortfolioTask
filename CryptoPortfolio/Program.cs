using CryptoPortfolio.Repositories;
using CryptoPortfolio.Repositories.Interfaces;
using CryptoPortfolio.Services;
using CryptoPortfolio.Services.Interfaces;
using CryptoPortfolio.Services.Timed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddScoped<ICryptoRepository, CoinLoreRepository>();
builder.Services.AddScoped<IPortfolioCalculation, PortfolioCalculationService>();
builder.Services.AddScoped<ILogging, LoggingService>();

builder.Services.AddHostedService<UpdateCryptoCurrencies>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
