using Microsoft.Extensions.Logging;

namespace CapturaBoleto.Application.Services;

/// <summary>
/// Serviço de logging estruturado para ações específicas do domínio de captura de boletos
/// </summary>
public class BoletoLoggingService
{
    private readonly ILogger<BoletoLoggingService> _logger;

    public BoletoLoggingService(ILogger<BoletoLoggingService> logger)
    {
        _logger = logger;
    }

    public void LogBoletoCreated(Guid boletoId, string codigoBarras, decimal valor, string beneficiario)
    {
        _logger.LogInformation(
            "Boleto criado com sucesso. " +
            "BoletoId: {BoletoId}, CodigoBarras: {CodigoBarras}, Valor: {Valor:C}, Beneficiario: {Beneficiario}",
            boletoId, 
            codigoBarras, 
            valor, 
            beneficiario);
    }

    public void LogBoletoValidationError(string codigoBarras, IEnumerable<string> errors)
    {
        _logger.LogWarning(
            "Falha na validação do boleto. CodigoBarras: {CodigoBarras}, Erros: {@Errors}",
            codigoBarras, 
            errors);
    }

    public void LogBoletoProcessed(Guid boletoId, string? observacoes = null)
    {
        _logger.LogInformation(
            "Boleto processado com sucesso. BoletoId: {BoletoId}, Observacoes: {Observacoes}",
            boletoId, 
            observacoes ?? "Nenhuma observação");
    }

    public void LogBoletoRejected(Guid boletoId, string motivo)
    {
        _logger.LogWarning(
            "Boleto rejeitado. BoletoId: {BoletoId}, Motivo: {Motivo}",
            boletoId, 
            motivo);
    }

    public void LogBoletoNotFound(Guid boletoId)
    {
        _logger.LogWarning(
            "Tentativa de acesso a boleto inexistente. BoletoId: {BoletoId}",
            boletoId);
    }

    public void LogDuplicateBoleto(string codigoBarras)
    {
        _logger.LogWarning(
            "Tentativa de criar boleto duplicado. CodigoBarras: {CodigoBarras}",
            codigoBarras);
    }

    public void LogRepositoryError(string operation, Guid? boletoId, Exception exception)
    {
        _logger.LogError(exception,
            "Erro no repositório durante operação. Operacao: {Operacao}, BoletoId: {BoletoId}",
            operation, 
            boletoId);
    }

    public void LogPerformanceMetric(string operation, TimeSpan elapsed, int? recordCount = null)
    {
        if (recordCount.HasValue)
        {
            _logger.LogInformation(
                "Métrica de performance. Operacao: {Operacao}, Tempo: {Elapsed:0.0000}ms, Registros: {RecordCount}",
                operation, 
                elapsed.TotalMilliseconds, 
                recordCount.Value);
        }
        else
        {
            _logger.LogInformation(
                "Métrica de performance. Operacao: {Operacao}, Tempo: {Elapsed:0.0000}ms",
                operation, 
                elapsed.TotalMilliseconds);
        }
    }

    public void LogApiRequestStart(string endpoint, string method, string? userId = null)
    {
        _logger.LogDebug(
            "Iniciando requisição da API. Endpoint: {Endpoint}, Method: {Method}, UserId: {UserId}",
            endpoint, 
            method, 
            userId ?? "Anônimo");
    }

    public void LogApiRequestEnd(string endpoint, string method, int statusCode, TimeSpan elapsed)
    {
        var logLevel = statusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
        
        _logger.Log(logLevel,
            "Finalizando requisição da API. Endpoint: {Endpoint}, Method: {Method}, StatusCode: {StatusCode}, Elapsed: {Elapsed:0.0000}ms",
            endpoint, 
            method, 
            statusCode, 
            elapsed.TotalMilliseconds);
    }

    public void LogBusinessRuleViolation(string rule, string details, Guid? boletoId = null)
    {
        _logger.LogWarning(
            "Violação de regra de negócio. Regra: {Regra}, Detalhes: {Detalhes}, BoletoId: {BoletoId}",
            rule, 
            details, 
            boletoId);
    }

    public void LogIntegrationEvent(string eventType, object eventData, bool success)
    {
        var logLevel = success ? LogLevel.Information : LogLevel.Error;
        
        _logger.Log(logLevel,
            "Evento de integração. Tipo: {EventType}, Sucesso: {Success}, Dados: {@EventData}",
            eventType, 
            success, 
            eventData);
    }
}