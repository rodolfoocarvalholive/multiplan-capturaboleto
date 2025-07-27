using CapturaBoleto.Domain.ValueObjects;

namespace CapturaBoleto.Domain.Entities;

/// <summary>
/// Entidade que representa um boleto bancário
/// </summary>
public class Boleto
{
    public Guid Id { get; private set; }
    public CodigoBarras CodigoBarras { get; private set; }
    public string LinhaDigitavel { get; private set; }
    public decimal Valor { get; private set; }
    public DateTime DataVencimento { get; private set; }
    public string Beneficiario { get; private set; }
    public Documento? DocumentoBeneficiario { get; private set; }
    public string? Pagador { get; private set; }
    public Documento? DocumentoPagador { get; private set; }
    public string? NossoNumero { get; private set; }
    public string? NumeroDocumento { get; private set; }
    public StatusBoleto Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataProcessamento { get; private set; }
    public string? ObservacoesProcessamento { get; private set; }

    public Boleto(
        CodigoBarras codigoBarras,
        decimal valor,
        DateTime dataVencimento,
        string beneficiario,
        Documento? documentoBeneficiario = null)
    {
        if (string.IsNullOrWhiteSpace(beneficiario))
            throw new ArgumentException("Beneficiário não pode ser nulo ou vazio", nameof(beneficiario));

        if (valor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero", nameof(valor));

        Id = Guid.NewGuid();
        CodigoBarras = codigoBarras ?? throw new ArgumentNullException(nameof(codigoBarras));
        LinhaDigitavel = codigoBarras.GetLinhaDigitavel();
        Valor = valor;
        DataVencimento = dataVencimento;
        Beneficiario = beneficiario;
        DocumentoBeneficiario = documentoBeneficiario;
        Status = StatusBoleto.Pendente;
        DataCriacao = DateTime.UtcNow;
    }

    public void DefinirPagador(string pagador, Documento? documento = null)
    {
        if (string.IsNullOrWhiteSpace(pagador))
            throw new ArgumentException("Pagador não pode ser nulo ou vazio", nameof(pagador));

        Pagador = pagador;
        DocumentoPagador = documento;
    }

    public void DefinirIdentificadores(string? nossoNumero, string? numeroDocumento)
    {
        NossoNumero = nossoNumero;
        NumeroDocumento = numeroDocumento;
    }

    public void MarcarComoProcessado(string? observacoes = null)
    {
        Status = StatusBoleto.Processado;
        DataProcessamento = DateTime.UtcNow;
        ObservacoesProcessamento = observacoes;
    }

    public void MarcarComoRejeitado(string observacoes)
    {
        if (string.IsNullOrWhiteSpace(observacoes))
            throw new ArgumentException("Observações são obrigatórias para rejeição", nameof(observacoes));

        Status = StatusBoleto.Rejeitado;
        DataProcessamento = DateTime.UtcNow;
        ObservacoesProcessamento = observacoes;
    }

    public bool EstaVencido()
    {
        return DataVencimento.Date < DateTime.Now.Date;
    }

    public int DiasParaVencimento()
    {
        return (DataVencimento.Date - DateTime.Now.Date).Days;
    }
}

public enum StatusBoleto
{
    Pendente,
    Processado,
    Rejeitado
}