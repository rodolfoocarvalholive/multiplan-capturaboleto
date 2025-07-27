using Serilog;
using Serilog.Events;

namespace CapturaBoleto.WebAPI.Configuration;

/// <summary>
/// Configurações do Serilog para logging estruturado
/// </summary>
public static class SerilogConfiguration
{
    /// <summary>
    /// Configura o Serilog como provedor de logging principal
    /// </summary>
    /// <param name="builder">WebApplicationBuilder</param>
    /// <returns>WebApplicationBuilder configurado</returns>
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        // Configurar Serilog early logging (para capturar logs de inicialização)
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        Log.Information("Configurando Serilog como provedor de logging principal");

        // Configurar Serilog como provedor de logging principal
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithThreadId()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/capturaboleto-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                fileSizeLimitBytes: 10_000_000,
                rollOnFileSizeLimit: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.Debug()
        );

        return builder;
    }

    /// <summary>
    /// Configura middleware de logging de requisições HTTP
    /// </summary>
    /// <param name="app">WebApplication</param>
    /// <returns>WebApplication configurado</returns>
    public static WebApplication UseSerilogRequestLoggingMiddleware(this WebApplication app)
    {
        Log.Information("Configurando middleware de logging de requisições HTTP");

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.GetLevel = (httpContext, elapsed, ex) => ex != null
                ? LogEventLevel.Error
                : httpContext.Response.StatusCode > 499
                    ? LogEventLevel.Error
                    : httpContext.Response.StatusCode > 399
                        ? LogEventLevel.Warning
                        : LogEventLevel.Information;
            
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
                diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
                
                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
                }
            };
        });

        return app;
    }

    /// <summary>
    /// Finaliza e limpa recursos do Serilog
    /// </summary>
    public static void CloseSerilogLogger()
    {
        Log.Information("Encerrando CapturaBoleto API");
        Log.CloseAndFlush();
    }
}