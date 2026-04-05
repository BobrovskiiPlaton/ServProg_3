using Greenswamp.Middleware;
using Greenswamp.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog для логирования
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/requests.log", 
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Добавление сервисов
builder.Services.AddRazorPages();
builder.Services.AddScoped<LoggingService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Настройка конвейера обработки запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Добавляем middleware для логирования запросов
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

app.MapRazorPages();

// Настройка обработки ошибки 404
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.Run();