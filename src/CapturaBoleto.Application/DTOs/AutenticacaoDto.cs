using System.ComponentModel.DataAnnotations;

namespace CapturaBoleto.Application.DTOs;

/// <summary>
/// DTO para autenticação do usuário para captura de boletos
/// </summary>
public class AutenticacaoDto
{
    /// <summary>
    /// Nome de usuário
    /// </summary>
    [Required(ErrorMessage = "Usuário é obrigatório")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Usuário deve ter entre 3 e 50 caracteres")]
    public string Usuario { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
    public string Senha { get; set; } = string.Empty;
}

/// <summary>
/// DTO de resposta da autenticação
/// </summary>
public class AutenticacaoResponseDto
{
    /// <summary>
    /// Indica se a autenticação foi bem-sucedida
    /// </summary>
    public bool Sucesso { get; set; }

    /// <summary>
    /// Nome do usuário autenticado
    /// </summary>
    public string? Usuario { get; set; }

    /// <summary>
    /// Token de sessão para usar nas próximas requisições
    /// </summary>
    public string? TokenSessao { get; set; }

    /// <summary>
    /// Data e hora da autenticação
    /// </summary>
    public DateTime? DataAutenticacao { get; set; }

    /// <summary>
    /// Data e hora de expiração do token
    /// </summary>
    public DateTime? DataExpiracaoToken { get; set; }

    /// <summary>
    /// Mensagem de erro (quando Sucesso = false)
    /// </summary>
    public string? MensagemErro { get; set; }
}

/// <summary>
/// DTO para iniciar captura de boleto
/// </summary>
public class IniciarCapturaDto
{
    /// <summary>
    /// Token de sessão obtido na autenticação
    /// </summary>
    [Required(ErrorMessage = "Token de sessão é obrigatório")]
    public string TokenSessao { get; set; } = string.Empty;

    /// <summary>
    /// Código de barras do boleto (44 dígitos)
    /// </summary>
    [Required(ErrorMessage = "Código de barras é obrigatório")]
    [StringLength(44, MinimumLength = 44, ErrorMessage = "Código de barras deve ter exatamente 44 dígitos")]
    [RegularExpression(@"^\d{44}$", ErrorMessage = "Código de barras deve conter apenas números")]
    public string CodigoBarras { get; set; } = string.Empty;
}

/// <summary>
/// DTO de resposta do início de captura
/// </summary>
public class IniciarCapturaResponseDto
{
    /// <summary>
    /// Indica se o início da captura foi bem-sucedido
    /// </summary>
    public bool Sucesso { get; set; }

    /// <summary>
    /// Código de barras validado
    /// </summary>
    public string? CodigoBarras { get; set; }

    /// <summary>
    /// Linha digitável correspondente
    /// </summary>
    public string? LinhaDigitavel { get; set; }

    /// <summary>
    /// ID da sessão de captura
    /// </summary>
    public string? SessaoId { get; set; }

    /// <summary>
    /// Informações básicas extraídas do código de barras
    /// </summary>
    public InformacoesBoletoBasicasDto? InformacoesBasicas { get; set; }

    /// <summary>
    /// Dados do boleto existente (se já estiver no sistema)
    /// </summary>
    public BoletoDto? BoletoExistente { get; set; }

    /// <summary>
    /// Mensagem de erro (quando Sucesso = false)
    /// </summary>
    public string? MensagemErro { get; set; }
}

/// <summary>
/// DTO com informações básicas do boleto extraídas do código de barras
/// </summary>
public class InformacoesBoletoBasicasDto
{
    /// <summary>
    /// Código do banco (3 dígitos)
    /// </summary>
    public string CodigoBanco { get; set; } = string.Empty;

    /// <summary>
    /// Código da moeda (1 dígito)
    /// </summary>
    public string CodigoMoeda { get; set; } = string.Empty;

    /// <summary>
    /// Dígito verificador (1 dígito)
    /// </summary>
    public string DigitoVerificador { get; set; } = string.Empty;

    /// <summary>
    /// Fator de vencimento (4 dígitos)
    /// </summary>
    public string FatorVencimento { get; set; } = string.Empty;

    /// <summary>
    /// Valor nominal (10 dígitos)
    /// </summary>
    public string ValorNominal { get; set; } = string.Empty;

    /// <summary>
    /// Data de vencimento calculada a partir do fator
    /// </summary>
    public DateTime? DataVencimentoCalculada { get; set; }

    /// <summary>
    /// Valor do boleto formatado em decimal
    /// </summary>
    public decimal? ValorFormatado 
    { 
        get 
        {
            if (decimal.TryParse(ValorNominal, out var valor))
                return valor / 100; // Valor em centavos
            return null;
        } 
    }

    /// <summary>
    /// Nome do banco baseado no código
    /// </summary>
    public string? NomeBanco 
    { 
        get 
        {
            return CodigoBanco switch
            {
                "001" => "Banco do Brasil",
                "033" => "Santander",
                "104" => "Caixa Econômica Federal",
                "237" => "Bradesco",
                "341" => "Itaú",
                "356" => "Banco Real",
                "389" => "Banco Mercantil",
                "422" => "Banco Safra",
                "748" => "Sicredi",
                _ => "Banco não identificado"
            };
        } 
    }
}