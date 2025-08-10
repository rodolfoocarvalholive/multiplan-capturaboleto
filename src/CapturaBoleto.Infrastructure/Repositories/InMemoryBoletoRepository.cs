using CapturaBoleto.Domain.Entities;
using CapturaBoleto.Domain.Repositories;

namespace CapturaBoleto.Infrastructure.Repositories;

/// <summary>
/// Implementação temporária em memória do repositório de boletos
/// </summary>
public class InMemoryBoletoRepository : IBoletoRepository
{
    private readonly List<Boleto> _boletos = new();

    public Task<Boleto?> ObterPorIdAsync(Guid id)
    {
        var boleto = _boletos.FirstOrDefault(b => b.Id == id);
        return Task.FromResult(boleto);
    }

    public Task<Boleto?> ObterPorCodigoBarrasAsync(string codigoBarras)
    {
        var boleto = _boletos.FirstOrDefault(b => b.CodigoBarras.Valor == codigoBarras);
        return Task.FromResult(boleto);
    }

    public Task<IEnumerable<Boleto>> ObterPorStatusAsync(StatusBoleto status)
    {
        var boletos = _boletos.Where(b => b.Status == status);
        return Task.FromResult(boletos);
    }

    public Task<IEnumerable<Boleto>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        var boletos = _boletos.Where(b => b.DataCriacao >= dataInicio && b.DataCriacao <= dataFim);
        return Task.FromResult(boletos);
    }

    public Task<IEnumerable<Boleto>> ObterVencidosAsync()
    {
        var boletos = _boletos.Where(b => b.EstaVencido());
        return Task.FromResult(boletos);
    }

    public Task AdicionarAsync(Boleto boleto)
    {
        _boletos.Add(boleto);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Boleto boleto)
    {
        var index = _boletos.FindIndex(b => b.Id == boleto.Id);
        if (index >= 0)
        {
            _boletos[index] = boleto;
        }
        return Task.CompletedTask;
    }

    public Task RemoverAsync(Guid id)
    {
        _boletos.RemoveAll(b => b.Id == id);
        return Task.CompletedTask;
    }

    public Task<bool> ExisteCodigoBarrasAsync(string codigoBarras)
    {
        var existe = _boletos.Any(b => b.CodigoBarras.Valor == codigoBarras);
        return Task.FromResult(existe);
    }

    public Task<int> ContarPorStatusAsync(StatusBoleto status)
    {
        var count = _boletos.Count(b => b.Status == status);
        return Task.FromResult(count);
    }
}