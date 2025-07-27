namespace CapturaBoleto.Application.DTOs;

/// <summary>
/// DTO de resposta com dados do boleto
/// </summary>
public class BoletoDto
{
    /// <summary>
    /// Identificador único do boleto
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código de barras do boleto
    /// </summary>
    public string CodigoBarras { get; set; } = string.Empty;

    /// <summary>
    /// Linha digitável do boleto
    /// </summary>
    public string LinhaDigitavel { get; set; } = string.Empty;

    /// <summary>
    /// Valor do boleto
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Data de vencimento
    /// </summary>
    public DateTime DataVencimento { get; set; }

    /// <summary>
    /// Nome do beneficiário
    /// </summary>
    public string Beneficiario { get; set; } = string.Empty;

    /// <summary>
    /// Documento do beneficiário formatado
    /// </summary>
    public string? DocumentoBeneficiario { get; set; }

    /// <summary>
    /// Nome do pagador
    /// </summary>
    public string? Pagador { get; set; }

    /// <summary>
    /// Documento do pagador formatado
    /// </summary>
    public string? DocumentoPagador { get; set; }

    /// <summary>
    /// Nosso número
    /// </summary>
    public string? NossoNumero { get; set; }

    /// <summary>
    /// Número do documento
    /// </summary>
    public string? NumeroDocumento { get; set; }

    /// <summary>
    /// Status atual do boleto
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime DataCriacao { get; set; }

    /// <summary>
    /// Data de processamento (se processado)
    /// </summary>
    public DateTime? DataProcessamento { get; set; }

    /// <summary>
    /// Observações do processamento
    /// </summary>
    public string? ObservacoesProcessamento { get; set; }

    /// <summary>
    /// Indica se o boleto está vencido
    /// </summary>
    public bool EstaVencido { get; set; }

    /// <summary>
    /// Dias para vencimento (negativo se vencido)
    /// </summary>
    public int DiasParaVencimento { get; set; }
}