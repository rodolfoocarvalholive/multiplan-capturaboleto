using Microsoft.Extensions.Logging;
using System.Text.Json;
using CapturaBoleto.Domain.Services;
using CapturaBoleto.Domain.Models;

namespace CapturaBoleto.Infrastructure.Services;

/// <summary>
/// Serviço para integração com APIs externas relacionadas a boletos
/// </summary>
public class BoletoExternalService : IBoletoExternalService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BoletoExternalService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private MultiplanConfigurationResponse? _multiplanConfig;

    public BoletoExternalService(HttpClient httpClient, ILogger<BoletoExternalService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    /// <summary>
    /// Carrega configurações da API do Multiplan
    /// </summary>
    private async Task<MultiplanConfigurationResponse?> CarregarConfiguracaoMultiplanAsync()
    {
        if (_multiplanConfig != null)
            return _multiplanConfig;

        try
        {
            _logger.LogInformation("Carregando configurações da API Multiplan");

            var configEndpoint = "https://canallojista.multiplan.com.br/assets/data/config.json";
            using var response = await _httpClient.GetAsync(configEndpoint);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                _multiplanConfig = JsonSerializer.Deserialize<MultiplanConfigurationResponse>(jsonContent, _jsonOptions);

                if (_multiplanConfig?.IsValid() == true)
                {
                    _logger.LogInformation("Configurações Multiplan carregadas com sucesso. Ambiente: {Ambiente}, Versão: {Versao}",
                        _multiplanConfig.Ambiente, _multiplanConfig.Versao);
                }
                else
                {
                    _logger.LogWarning("Configurações Multiplan inválidas");
                    _multiplanConfig = null;
                }
            }
            else
            {
                _logger.LogWarning("Falha ao carregar configurações Multiplan. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar configurações da API Multiplan");
        }

        return _multiplanConfig;
    }

    /// <summary>
    /// Consulta informações detalhadas de um boleto em APIs externas de bancos
    /// </summary>
    /// <param name="codigoBarras">Código de barras do boleto</param>
    /// <param name="codigoBanco">Código do banco emissor</param>
    /// <returns>Informações detalhadas do boleto ou null se não encontrado</returns>
    public async Task<BoletoExternalInfo?> ConsultarBoletoAsync(string codigoBarras, string codigoBanco)
    {
        try
        {
            _logger.LogInformation("Iniciando consulta externa do boleto. CodigoBarras: {CodigoBarras}, Banco: {CodigoBanco}",
                codigoBarras, codigoBanco);

            // Carregar configurações do Multiplan se necessário
            var multiplanConfig = await CarregarConfiguracaoMultiplanAsync();
            
            // TODO: Implementar chamada para API externa específica do banco
            // Diferentes bancos podem ter diferentes endpoints e formatos
            var endpoint = ObterEndpointPorBanco(codigoBanco);
            
            // Se não tiver endpoint configurado para o banco, usar API do Multiplan como fallback
            if (string.IsNullOrEmpty(endpoint) && multiplanConfig != null)
            {
                endpoint = multiplanConfig.GetFullApiUrl("boletos");
                _logger.LogInformation("Usando endpoint do Multiplan como fallback: {Endpoint}", endpoint);
            }

            if (string.IsNullOrEmpty(endpoint))
            {
                _logger.LogWarning("Endpoint não configurado para o banco: {CodigoBanco}", codigoBanco);
                return null;
            }

            var requestUri = $"{endpoint}/{codigoBarras}";

            // Adicionar header de API Key se disponível
            if (multiplanConfig != null && !string.IsNullOrEmpty(multiplanConfig.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Remove("X-API-Key");
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", multiplanConfig.ApiKey);
            }

            using var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var boletoInfo = JsonSerializer.Deserialize<BoletoExternalInfo>(jsonContent, _jsonOptions);

                _logger.LogInformation("Consulta externa bem-sucedida para boleto: {CodigoBarras}", codigoBarras);
                return boletoInfo;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Boleto não encontrado na API externa: {CodigoBarras}", codigoBarras);
                return null;
            }

            _logger.LogWarning("Falha na consulta externa. StatusCode: {StatusCode}, CodigoBarras: {CodigoBarras}",
                response.StatusCode, codigoBarras);
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de rede na consulta externa do boleto: {CodigoBarras}", codigoBarras);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout na consulta externa do boleto: {CodigoBarras}", codigoBarras);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao deserializar resposta da API externa: {CodigoBarras}", codigoBarras);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado na consulta externa do boleto: {CodigoBarras}", codigoBarras);
            return null;
        }
    }

    /// <summary>
    /// Valida se um boleto está ativo e pode ser pago
    /// </summary>
    /// <param name="codigoBarras">Código de barras do boleto</param>
    /// <returns>Status de validação do boleto</returns>
    public async Task<BoletoValidationStatus> ValidarStatusBoletoAsync(string codigoBarras)
    {
        try
        {
            _logger.LogInformation("Iniciando validação de status do boleto: {CodigoBarras}", codigoBarras);

            // Extrair código do banco do código de barras
            var codigoBanco = codigoBarras[..3];
            var endpoint = ObterEndpointPorBanco(codigoBanco);

            if (string.IsNullOrEmpty(endpoint))
            {
                return new BoletoValidationStatus
                {
                    IsValid = false,
                    Status = "BANCO_NAO_SUPORTADO",
                    Message = $"Banco {codigoBanco} não possui integração configurada"
                };
            }

            var requestUri = $"{endpoint}/boletos/{codigoBarras}/status";

            using var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var validationStatus = JsonSerializer.Deserialize<BoletoValidationStatus>(jsonContent, _jsonOptions);

                _logger.LogInformation("Validação de status bem-sucedida. CodigoBarras: {CodigoBarras}, Status: {Status}",
                    codigoBarras, validationStatus?.Status);

                return validationStatus ?? new BoletoValidationStatus { IsValid = false, Status = "ERRO_DESCONHECIDO" };
            }

            return new BoletoValidationStatus
            {
                IsValid = false,
                Status = "API_INDISPONIVEL",
                Message = $"API do banco retornou status: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na validação de status do boleto: {CodigoBarras}", codigoBarras);

            return new BoletoValidationStatus
            {
                IsValid = false,
                Status = "ERRO_INTERNO",
                Message = ex.Message
            };
        }
    }

    /// <summary>
    /// Simula o pagamento de um boleto em ambiente de desenvolvimento/teste
    /// </summary>
    /// <param name="codigoBarras">Código de barras do boleto</param>
    /// <param name="valorPagamento">Valor do pagamento</param>
    /// <returns>Resultado da simulação de pagamento</returns>
    public async Task<PaymentSimulationResult> SimularPagamentoAsync(string codigoBarras, decimal valorPagamento)
    {
        try
        {
            _logger.LogInformation("Simulando pagamento de boleto. CodigoBarras: {CodigoBarras}, Valor: {Valor:C}",
                codigoBarras, valorPagamento);

            // Simular delay de processamento
            await Task.Delay(Random.Shared.Next(500, 2000));

            // Simulação de diferentes cenários de pagamento
            var scenarios = new[]
            {
                new PaymentSimulationResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Message = "Pagamento processado com sucesso" },
                new PaymentSimulationResult { Success = false, TransactionId = null, Message = "Saldo insuficiente" },
                new PaymentSimulationResult { Success = false, TransactionId = null, Message = "Boleto vencido" },
                new PaymentSimulationResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Message = "Pagamento processado com juros" }
            };

            // 80% de chance de sucesso em simulação
            var result = Random.Shared.NextDouble() < 0.8 ? scenarios[0] : scenarios[Random.Shared.Next(1, scenarios.Length)];

            _logger.LogInformation("Simulação de pagamento finalizada. CodigoBarras: {CodigoBarras}, Sucesso: {Sucesso}",
                codigoBarras, result.Success);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na simulação de pagamento: {CodigoBarras}", codigoBarras);

            return new PaymentSimulationResult
            {
                Success = false,
                TransactionId = null,
                Message = $"Erro interno: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtém o endpoint da API externa baseado no código do banco
    /// </summary>
    private static string? ObterEndpointPorBanco(string codigoBanco)
    {
        // TODO: Configurar endpoints reais dos bancos
        // Em produção, estes endpoints devem vir de configuração
        return codigoBanco switch
        {
            "001" => "https://api.bb.com.br/boletos", // Banco do Brasil
            "033" => "https://api.santander.com.br/boletos", // Santander
            "104" => "https://api.caixa.gov.br/boletos", // Caixa
            "237" => "https://api.bradesco.com.br/boletos", // Bradesco
            "341" => "https://api.itau.com.br/boletos", // Itaú
            _ => null // Banco não suportado
        };
    }
}