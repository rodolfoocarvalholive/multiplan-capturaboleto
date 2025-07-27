using CapturaBoleto.Application.Services;
using Serilog;

namespace CapturaBoleto.WebAPI.Configuration;

/// <summary>
/// Configuração de registro de serviços de injeção de dependência
/// </summary>
public static class DependencyInjectionConfiguration
{
    /// <summary>
    /// Registra todos os serviços da aplicação
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="configuration">IConfiguration</param>
    /// <returns>IServiceCollection configurado</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Information("Registrando serviços da aplicação");

        // Registrar serviços de logging personalizados
        services.AddScoped<BoletoLoggingService>();

        // Registrar serviços de domínio
        services.AddDomainServices();

        // Registrar serviços de aplicação
        services.AddApplicationLayerServices();

        // Registrar serviços de infraestrutura
        services.AddInfrastructureServices(configuration);

        // Registrar controllers
        services.AddControllers(options =>
        {
            // Configurações globais dos controllers
            options.SuppressAsyncSuffixInActionNames = false; // Manter sufixo Async
        });

        Log.Information("Serviços da aplicação registrados com sucesso");

        return services;
    }

    /// <summary>
    /// Registra serviços da camada de domínio
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection configurado</returns>
    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        Log.Debug("Registrando serviços da camada de domínio");

        // Registrar serviços de domínio aqui quando necessário
        // services.AddScoped<IBoletoValidationService, BoletoValidationService>();

        return services;
    }

    /// <summary>
    /// Registra serviços da camada de aplicação
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection configurado</returns>
    private static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        Log.Debug("Registrando serviços da camada de aplicação");

        // Registrar casos de uso (Use Cases) quando implementados
        // services.AddScoped<ICriarBoletoUseCase, CriarBoletoUseCase>();
        // services.AddScoped<IProcessarBoletoUseCase, ProcessarBoletoUseCase>();

        // Registrar AutoMapper quando necessário
        // services.AddAutoMapper(typeof(BoletoMappingProfile));

        return services;
    }

    /// <summary>
    /// Registra serviços da camada de infraestrutura
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="configuration">IConfiguration</param>
    /// <returns>IServiceCollection configurado</returns>
    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Debug("Registrando serviços da camada de infraestrutura");

        // Registrar contexto do Entity Framework quando implementado
        // services.AddDbContext<CapturaBoletoDbContext>(options =>
        //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Registrar repositórios
        // services.AddScoped<IBoletoRepository, BoletoRepository>();

        // Registrar serviços externos
        // services.AddHttpClient<IBancoCentralService, BancoCentralService>();

        return services;
    }

    /// <summary>
    /// Configura políticas de CORS
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection configurado</returns>
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        Log.Information("Configurando políticas de CORS");

        services.AddCors(options =>
        {
            options.AddPolicy("CapturaBoletoPolicy", policy =>
            {
                policy.WithOrigins(
                        "http://localhost:3000",    // React dev server
                        "http://localhost:4200",    // Angular dev server
                        "https://localhost:5001",   // Local HTTPS
                        "https://capturaboleto.multiplan.com.br" // Produção
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });

            // Política mais restritiva para produção
            options.AddPolicy("ProductionPolicy", policy =>
            {
                policy.WithOrigins("https://capturaboleto.multiplan.com.br")
                    .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
                    .WithHeaders("Content-Type", "Authorization")
                    .AllowCredentials();
            });
        });

        return services;
    }

    /// <summary>
    /// Configura cache em memória
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection configurado</returns>
    public static IServiceCollection AddCacheConfiguration(this IServiceCollection services)
    {
        Log.Information("Configurando cache em memória");

        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 100; // Limite de 100 entradas
        });

        // Configurar cache distribuído quando necessário
        // services.AddStackExchangeRedisCache(options =>
        // {
        //     options.Configuration = configuration.GetConnectionString("Redis");
        // });

        return services;
    }

    /// <summary>
    /// Configura compressão de resposta
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection configurado</returns>
    public static IServiceCollection AddCompressionConfiguration(this IServiceCollection services)
    {
        Log.Information("Configurando compressão de resposta");

        services.AddResponseCompression(options =>
        {
            options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
            options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
            options.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults
                .MimeTypes.Concat(new[] { "application/json" });
        });

        return services;
    }
}