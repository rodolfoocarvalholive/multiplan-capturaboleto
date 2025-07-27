using System.Reflection;
using Serilog;
using Serilog.Events;
using CapturaBoleto.Application.Services;

// Configurar Serilog early logging (para capturar logs de inicialização)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando CapturaBoleto API");

    var builder = WebApplication.CreateBuilder(args);

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

    // Add services to the container.
    builder.Services.AddControllers();

    // Registrar serviços de logging personalizados
    builder.Services.AddScoped<BoletoLoggingService>();

    // Configure Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "CapturaBoleto API",
            Version = "v1",
            Description = "API para captura e processamento de boletos bancários com logging estruturado Serilog",
            Contact = new()
            {
                Name = "Equipe Desenvolvimento DG Multiplan",
                Email = "dev@multiplan.com.br"
            }
        });

        // Include XML comments if available
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });

    // Registrar serviços de domínio e aplicação
    // TODO: Implementar injeção de dependência para repositórios e services

    var app = builder.Build();

    // Configurar middleware de logging de requisições
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

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CapturaBoleto API v1");
            c.RoutePrefix = string.Empty; // Swagger UI at app's root
            c.DocumentTitle = "CapturaBoleto API - Documentação";
        });

        Log.Information("Swagger habilitado para ambiente de desenvolvimento");
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("CapturaBoleto API configurada com sucesso");
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro fatal ao inicializar a aplicação");
}
finally
{
    Log.Information("Encerrando CapturaBoleto API");
    Log.CloseAndFlush();
}
