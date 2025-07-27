using System.Reflection;
using Serilog;
using Serilog.Events;
using CapturaBoleto.Application.Services;
using CapturaBoleto.WebAPI.Configuration;

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

    // ===== CONFIGURAÇÃO DE LOGGING =====
    builder.ConfigureSerilog();

    // ===== CONFIGURAÇÃO DE SERVIÇOS =====
    builder.Services
        .AddApplicationServices(builder.Configuration)
        .AddSwaggerConfiguration()
        .AddCorsConfiguration()
        .AddCacheConfiguration()
        .AddCompressionConfiguration();

    // ===== CONSTRUÇÃO DA APLICAÇÃO =====
    var app = builder.Build();

    // ===== CONFIGURAÇÃO DO PIPELINE DE MIDDLEWARE =====
    app.ConfigureMiddlewarePipeline();

    Log.Information("CapturaBoleto API configurada com sucesso");
    
    // ===== EXECUÇÃO DA APLICAÇÃO =====
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro fatal ao inicializar a aplicação");
    return 1; // Código de saída para indicar erro
}
finally
{
    SerilogConfiguration.CloseSerilogLogger();
}

return 0; // Código de saída de sucesso
