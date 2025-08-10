namespace CapturaBoleto.Domain.Models;

/// <summary>
/// Informações detalhadas de um boleto obtidas de APIs externas
/// </summary>
public class BoletoExternalInfo
{
    public string CodigoBarras { get; set; } = string.Empty;
    public string LinhaDigitavel { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public string Beneficiario { get; set; } = string.Empty;
    public string DocumentoBeneficiario { get; set; } = string.Empty;
    public string? Pagador { get; set; }
    public string? DocumentoPagador { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? JurosAtraso { get; set; }
    public decimal? MultaAtraso { get; set; }
    public decimal? DescontoAntecipacao { get; set; }
    public DateTime? DataLimitePagamento { get; set; }
    public string? InstrucoesPagamento { get; set; }
    public Dictionary<string, object>? DadosAdicionais { get; set; }
}

/// <summary>
/// Status de validação de um boleto
/// </summary>
public class BoletoValidationStatus
{
    public bool IsValid { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DateTime? DataConsulta { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Resultado da simulação de pagamento
/// </summary>
public class PaymentSimulationResult
{
    public bool Success { get; set; }
    public string? TransactionId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}