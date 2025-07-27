using Serilog;

namespace CapturaBoleto.WebAPI.Configuration;

/// <summary>
/// Configuração do pipeline de middleware da aplicação
/// </summary>
public static class MiddlewareConfiguration
{
    /// <summary>
    /// Configura o pipeline de middleware da aplicação
    /// </summary>
    /// <param name="app">WebApplication</param>
    /// <returns>WebApplication configurado</returns>
    public static WebApplication ConfigureMiddlewarePipeline(this WebApplication app)
    {
        // Middleware específicos por ambiente
        if (app.Environment.IsDevelopment())
        {
            app.UseDevelopmentMiddleware();
        }
        else
        {
            app.UseProductionMiddleware();
        }

        // Middleware de segurança
        app.UseSecurityMiddleware();

        // Middleware de logging de requisições (Serilog)
        app.UseSerilogRequestLoggingMiddleware();

        // Middleware de compressão
        app.UseResponseCompression();

        // Middleware de roteamento e autorização
        app.UseHttpsRedirection();
        app.UseCors("CapturaBoletoPolicy");
        app.UseAuthentication(); // Para futuras implementações
        app.UseAuthorization();

        // Middleware de controllers
        app.MapControllers();

        return app;
    }

    /// <summary>
    /// Configura middleware específicos para desenvolvimento
    /// </summary>
    /// <param name="app">WebApplication</param>
    /// <returns>WebApplication configurado</returns>
    private static WebApplication UseDevelopmentMiddleware(this WebApplication app)
    {
        // Página de exceção para desenvolvedores
        app.UseDeveloperExceptionPage();

        // Swagger UI
        app.UseSwaggerConfiguration();

        return app;
    }

    /// <summary>
    /// Configura middleware específicos para produção
    /// </summary>
    /// <param name="app">WebApplication</param>
    /// <returns>WebApplication configurado</returns>
    private static WebApplication UseProductionMiddleware(this WebApplication app)
    {
        // Página de erro genérica
        app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    message = "Erro interno do servidor",
                    timestamp = DateTime.UtcNow,
                    traceId = context.TraceIdentifier
                };

                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
            });
        });

        // HSTS (HTTP Strict Transport Security)
        app.UseHsts();

        return app;
    }

    /// <summary>
    /// Configura middleware de segurança
    /// </summary>
    /// <param name="app">WebApplication</param>
    /// <returns>WebApplication configurado</returns>
    private static WebApplication UseSecurityMiddleware(this WebApplication app)
    {
        // Headers de segurança
        app.Use(async (context, next) =>
        {
            // X-Frame-Options
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            
            // X-Content-Type-Options
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            
            // X-XSS-Protection
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            
            // Referrer-Policy
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            
            // Content-Security-Policy (básico)
            context.Response.Headers.Append("Content-Security-Policy", 
                "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'");

            await next();
        });

        return app;
    }

    /// <summary>
    /// Configura middleware de tratamento de erros global
    /// </summary>
    /// <param name="app">WebApplication</param>
    /// <returns>WebApplication configurado</returns>
    public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        return app;
    }
}

/// <summary>
/// Middleware global para tratamento de exceções
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado na requisição {RequestPath}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ArgumentException => 400,
            UnauthorizedAccessException => 401,
            NotImplementedException => 501,
            _ => 500
        };

        var response = new
        {
            message = context.Response.StatusCode switch
            {
                400 => "Dados inválidos fornecidos",
                401 => "Não autorizado",
                501 => "Funcionalidade não implementada",
                _ => "Erro interno do servidor"
            },
            statusCode = context.Response.StatusCode,
            timestamp = DateTime.UtcNow,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
}