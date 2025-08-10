using System.Text.Json.Serialization;

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

/// <summary>
/// Configurações da API externa Multiplan Canal Lojista
/// Representa a resposta da API de configurações que contém URLs, chaves e parâmetros necessários
/// </summary>
public class MultiplanConfigurationResponse
{
    /// <summary>
    /// URL base da API do Canal Lojista
    /// </summary>
    [JsonPropertyName("API_URL")]
    public string ApiUrl { get; set; } = string.Empty;

    /// <summary>
    /// Chave de API para autenticação
    /// </summary>
    [JsonPropertyName("API_KEY")]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// URI para notificações
    /// </summary>
    [JsonPropertyName("NOTIF_URI")]
    public string NotificationUri { get; set; } = string.Empty;

    /// <summary>
    /// Chave do storage
    /// </summary>
    [JsonPropertyName("STORAGE_KEY")]
    public string StorageKey { get; set; } = string.Empty;

    /// <summary>
    /// Storage para notificações
    /// </summary>
    [JsonPropertyName("STORAGE_NOTIFICACAO")]
    public string StorageNotificacao { get; set; } = string.Empty;

    /// <summary>
    /// ID do Application Insights
    /// </summary>
    [JsonPropertyName("INSIGHTS")]
    public string Insights { get; set; } = string.Empty;

    /// <summary>
    /// Ambiente de execução (p = produção, h = homologação, d = desenvolvimento)
    /// </summary>
    [JsonPropertyName("AMBIENTE")]
    public string Ambiente { get; set; } = string.Empty;

    /// <summary>
    /// Versão da API
    /// </summary>
    [JsonPropertyName("VERSAO")]
    public string Versao { get; set; } = string.Empty;

    /// <summary>
    /// URL do chat de atendimento
    /// </summary>
    [JsonPropertyName("CHAT_URL")]
    public string ChatUrl { get; set; } = string.Empty;

    /// <summary>
    /// Chave do Pusher para notificações em tempo real
    /// </summary>
    [JsonPropertyName("PUSHER_KEY")]
    public string PusherKey { get; set; } = string.Empty;

    /// <summary>
    /// Ambiente do Pusher
    /// </summary>
    [JsonPropertyName("PUSHER_AMBIENTE")]
    public string PusherAmbiente { get; set; } = string.Empty;

    /// <summary>
    /// Canal do Pusher
    /// </summary>
    [JsonPropertyName("PUSHER_CHANNEL")]
    public string PusherChannel { get; set; } = string.Empty;

    /// <summary>
    /// Cluster do Pusher
    /// </summary>
    [JsonPropertyName("PUSHER_CLUSTER")]
    public string PusherCluster { get; set; } = string.Empty;

    /// <summary>
    /// URLs das imagens do carrossel da home
    /// </summary>
    [JsonPropertyName("MULTIPLAN_IMAGE_HOME")]
    public List<string> MultiplanImageHome { get; set; } = new();

    /// <summary>
    /// URLs de redirecionamento das imagens da home
    /// </summary>
    [JsonPropertyName("MULTIPLAN_URL_HOME")]
    public List<string> MultiplanUrlHome { get; set; } = new();

    /// <summary>
    /// URL da imagem do banner da home
    /// </summary>
    [JsonPropertyName("MULTIPLAN_IMAGE_HOME_BANNER")]
    public string MultiplanImageHomeBanner { get; set; } = string.Empty;

    /// <summary>
    /// URLs das imagens da tela de login
    /// </summary>
    [JsonPropertyName("MULTIPLAN_IMAGE_LOGIN")]
    public List<string> MultiplanImageLogin { get; set; } = new();

    /// <summary>
    /// URL da imagem da tela de recuperação de senha
    /// </summary>
    [JsonPropertyName("MULTIPLAN_IMAGE_SENHA")]
    public string MultiplanImageSenha { get; set; } = string.Empty;

    /// <summary>
    /// URL da nova imagem da home
    /// </summary>
    [JsonPropertyName("MULTIPLAN_IMAGE_NEW")]
    public string MultiplanImageNew { get; set; } = string.Empty;

    /// <summary>
    /// ID da aplicação Azure AD
    /// </summary>
    [JsonPropertyName("APPID")]
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// ID do tenant Azure AD
    /// </summary>
    [JsonPropertyName("TENANT")]
    public string Tenant { get; set; } = string.Empty;

    /// <summary>
    /// Nome do pacote da aplicação Android
    /// </summary>
    [JsonPropertyName("PACKAGE_NAME")]
    public string PackageName { get; set; } = string.Empty;

    /// <summary>
    /// Hash de validação para aplicação Android (produção)
    /// </summary>
    [JsonPropertyName("HASH_ANDROID")]
    public string HashAndroid { get; set; } = string.Empty;

    /// <summary>
    /// Hash de validação para aplicação Android (desenvolvimento)
    /// </summary>
    [JsonPropertyName("HASH_ANDROID_DEV")]
    public string HashAndroidDev { get; set; } = string.Empty;

    /// <summary>
    /// Verifica se a configuração está definida para ambiente de produção
    /// </summary>
    public bool IsProduction => string.Equals(Ambiente, "p", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Verifica se a configuração está definida para ambiente de homologação
    /// </summary>
    public bool IsHomologation => string.Equals(Ambiente, "h", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Verifica se a configuração está definida para ambiente de desenvolvimento
    /// </summary>
    public bool IsDevelopment => string.Equals(Ambiente, "d", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Valida se todas as configurações obrigatórias estão preenchidas
    /// </summary>
    /// <returns>True se todas as configurações obrigatórias estão presentes</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(ApiUrl) &&
               !string.IsNullOrWhiteSpace(ApiKey) &&
               !string.IsNullOrWhiteSpace(Ambiente) &&
               !string.IsNullOrWhiteSpace(Versao);
    }

    /// <summary>
    /// Obtém a URL completa para um endpoint específico
    /// </summary>
    /// <param name="endpoint">Endpoint relativo</param>
    /// <returns>URL completa do endpoint</returns>
    public string GetFullApiUrl(string endpoint)
    {
        if (string.IsNullOrWhiteSpace(ApiUrl))
            return string.Empty;

        var baseUrl = ApiUrl.TrimEnd('/');
        var cleanEndpoint = endpoint.TrimStart('/');
        
        return $"{baseUrl}/{cleanEndpoint}";
    }
}