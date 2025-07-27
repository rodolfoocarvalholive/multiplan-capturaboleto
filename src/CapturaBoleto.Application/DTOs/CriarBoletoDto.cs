using System.ComponentModel.DataAnnotations;

namespace CapturaBoleto.Application.DTOs;

/// <summary>
/// DTO para criação de um novo boleto
/// </summary>
public class CriarBoletoDto
{
    /// <summary>
    /// Código de barras do boleto (44 dígitos)
    /// </summary>
    [Required(ErrorMessage = "Código de barras é obrigatório")]
    [StringLength(44, MinimumLength = 44, ErrorMessage = "Código de barras deve ter exatamente 44 dígitos")]
    [RegularExpression(@"^\d{44}$", ErrorMessage = "Código de barras deve conter apenas números")]
    public string CodigoBarras { get; set; } = string.Empty;

    /// <summary>
    /// Valor do boleto
    /// </summary>
    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, 999999.99, ErrorMessage = "Valor deve estar entre R$ 0,01 e R$ 999.999,99")]
    public decimal Valor { get; set; }

    /// <summary>
    /// Data de vencimento do boleto
    /// </summary>
    [Required(ErrorMessage = "Data de vencimento é obrigatória")]
    public DateTime DataVencimento { get; set; }

    /// <summary>
    /// Nome do beneficiário
    /// </summary>
    [Required(ErrorMessage = "Beneficiário é obrigatório")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Beneficiário deve ter entre 3 e 200 caracteres")]
    public string Beneficiario { get; set; } = string.Empty;

    /// <summary>
    /// Documento do beneficiário (CPF ou CNPJ)
    /// </summary>
    public string? DocumentoBeneficiario { get; set; }

    /// <summary>
    /// Nome do pagador
    /// </summary>
    [StringLength(200, ErrorMessage = "Pagador deve ter no máximo 200 caracteres")]
    public string? Pagador { get; set; }

    /// <summary>
    /// Documento do pagador (CPF ou CNPJ)
    /// </summary>
    public string? DocumentoPagador { get; set; }

    /// <summary>
    /// Nosso número do boleto
    /// </summary>
    [StringLength(50, ErrorMessage = "Nosso número deve ter no máximo 50 caracteres")]
    public string? NossoNumero { get; set; }

    /// <summary>
    /// Número do documento
    /// </summary>
    [StringLength(50, ErrorMessage = "Número do documento deve ter no máximo 50 caracteres")]
    public string? NumeroDocumento { get; set; }
}