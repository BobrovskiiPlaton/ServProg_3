using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", 
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 31,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddRazorPages();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    
    Log.Information("Incoming request: {Method} {Path} | IP: {RemoteIpAddress} | UserAgent: {UserAgent}",
        context.Request.Method,
        context.Request.Path,
        context.Connection.RemoteIpAddress,
        context.Request.Headers["User-Agent"].ToString());
    
    await next();
    
    stopwatch.Stop();
    
    Log.Information("Outgoing response: {Method} {Path} | StatusCode: {StatusCode} | Duration: {Duration}ms",
        context.Request.Method,
        context.Request.Path,
        context.Response.StatusCode,
        stopwatch.ElapsedMilliseconds);
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.Run();