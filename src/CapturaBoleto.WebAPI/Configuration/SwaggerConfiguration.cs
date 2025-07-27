using System.Reflection;
using Serilog;

namespace CapturaBoleto.WebAPI.Configuration;

/// <summary>
/// Configurações do Swagger/OpenAPI
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Configura serviços do Swagger/OpenAPI
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection configurado</returns>
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        Log.Information("Configurando Swagger/OpenAPI");

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
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
                },
                License = new()
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Configurar autorização no Swagger (para futuras implementações)
            // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            // {
            //     Description = "JWT Authorization header usando o esquema Bearer.",
            //     Name = "Authorization",
            //     In = ParameterLocation.Header,
            //     Type = SecuritySchemeType.ApiKey,
            //     Scheme = "Bearer"
            // });

            // Include XML comments if available
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
                Log.Information("Comentários XML incluídos no Swagger: {XmlPath}", xmlPath);
            }
            else
            {
                Log.Warning("Arquivo de comentários XML não encontrado: {XmlPath}", xmlPath);
            }

            // Configurar filtros personalizados do Swagger
            c.EnableAnnotations(); // Habilita anotações do Swagger
            c.DescribeAllParametersInCamelCase(); // Parâmetros em camelCase
        });

        return services;
    }

    /// <summary>
    /// Configura middleware do Swagger UI
    /// </summary>
    /// <param name="app">WebApplication</param>
    /// <returns>WebApplication configurado</returns>
    public static WebApplication UseSwaggerConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            Log.Information("Swagger habilitado para ambiente de desenvolvimento");

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CapturaBoleto API v1");
                c.RoutePrefix = string.Empty; // Swagger UI na raiz da aplicação
                c.DocumentTitle = "CapturaBoleto API - Documentação";
                c.DefaultModelsExpandDepth(-1); // Ocultar modelos por padrão
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Não expandir operações
                c.EnableDeepLinking(); // Habilitar deep linking
                c.EnableFilter(); // Habilitar filtro de busca
                c.ShowExtensions(); // Mostrar extensões
                c.ShowCommonExtensions(); // Mostrar extensões comuns
                c.EnableValidator(); // Habilitar validador de schema
            });
        }
        else
        {
            Log.Information("Swagger desabilitado para ambiente de produção");
        }

        return app;
    }
}