using CapturaBoleto.Domain.Entities;

namespace CapturaBoleto.Domain.Repositories;

/// <summary>
/// Interface do repositório de boletos
/// </summary>
public interface IBoletoRepository
{
    Task<Boleto?> ObterPorIdAsync(Guid id);
    Task<Boleto?> ObterPorCodigoBarrasAsync(string codigoBarras);
    Task<IEnumerable<Boleto>> ObterPorStatusAsync(StatusBoleto status);
    Task<IEnumerable<Boleto>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<Boleto>> ObterVencidosAsync();
    Task AdicionarAsync(Boleto boleto);
    Task AtualizarAsync(Boleto boleto);
    Task RemoverAsync(Guid id);
    Task<bool> ExisteCodigoBarrasAsync(string codigoBarras);
    Task<int> ContarPorStatusAsync(StatusBoleto status);
}