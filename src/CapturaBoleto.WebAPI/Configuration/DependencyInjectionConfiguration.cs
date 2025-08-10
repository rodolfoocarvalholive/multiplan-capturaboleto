using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using CapturaBoleto.Application.Services;
using CapturaBoleto.Domain.Services;
using CapturaBoleto.Infrastructure.Services;
using CapturaBoleto.Domain.Repositories;
using CapturaBoleto.Infrastructure.Repositories;

namespace CapturaBoleto.WebAPI.Configuration;

/// <summary>
/// Configurações de Dependency Injection e serviços da aplicação
/// </summary>
public static class DependencyInjectionConfiguration
{
    /// <summary>
    /// Adiciona serviços da camada de aplicação
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Serviços da aplicação
        services.AddScoped<BoletoLoggingService>();

        // Serviços de domínio
        services.AddScoped<CapturaBoletoService>();

        // Serviços de domínio e infraestrutura
        services.AddScoped<IBoletoExternalService, BoletoExternalService>();
        services.AddScoped<IBoletoRepository, InMemoryBoletoRepository>();

        // HttpClient para BoletoExternalService
        services.AddHttpClient<BoletoExternalService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "CapturaBoleto/1.0.0");
        });

        // Configurações básicas do ASP.NET Core
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        return services;
    }

    /// <summary>
    /// Configura CORS para desenvolvimento
    /// </summary>
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CapturaBoletoPolicy", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

    /// <summary>
    /// Configura cache em memória
    /// </summary>
    public static IServiceCollection AddCacheConfiguration(this IServiceCollection services)
    {
        services.AddMemoryCache();
        return services;
    }

    /// <summary>
    /// Configura compressão de resposta
    /// </summary>
    public static IServiceCollection AddCompressionConfiguration(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
            options.Providers.Add<BrotliCompressionProvider>();
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        return services;
    }
}