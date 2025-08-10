using CapturaBoleto.Domain.Models;

namespace CapturaBoleto.Domain.Services;

/// <summary>
/// Interface para integração com APIs externas de boletos
/// </summary>
public interface IBoletoExternalService
{
    /// <summary>
    /// Consulta informações detalhadas de um boleto em APIs externas de bancos
    /// </summary>
    /// <param name="codigoBarras">Código de barras do boleto</param>
    /// <param name="codigoBanco">Código do banco emissor</param>
    /// <returns>Informações detalhadas do boleto ou null se não encontrado</returns>
    Task<BoletoExternalInfo?> ConsultarBoletoAsync(string codigoBarras, string codigoBanco);

    /// <summary>
    /// Valida se um boleto está ativo e pode ser pago
    /// </summary>
    /// <param name="codigoBarras">Código de barras do boleto</param>
    /// <returns>Status de validação do boleto</returns>
    Task<BoletoValidationStatus> ValidarStatusBoletoAsync(string codigoBarras);

    /// <summary>
    /// Simula o pagamento de um boleto em ambiente de desenvolvimento/teste
    /// </summary>
    /// <param name="codigoBarras">Código de barras do boleto</param>
    /// <param name="valorPagamento">Valor do pagamento</param>
    /// <returns>Resultado da simulação de pagamento</returns>
    Task<PaymentSimulationResult> SimularPagamentoAsync(string codigoBarras, decimal valorPagamento);
}