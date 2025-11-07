using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;


namespace Flowenter.Api.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        var logPath = builder.Configuration["Serilog:LogPath"] ?? "Logs/flowenter-.log";
        var minLogLevel = Enum.Parse<LogEventLevel>(
            builder.Configuration["Serilog:MinimumLevel:Default"] ?? "Information");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(minLogLevel)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", "Flowenter.Api")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                new CompactJsonFormatter(),
                path: builder.Configuration["Serilog:JsonLogPath"] ?? "Logs/flowenter-.json",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30)
            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }
}