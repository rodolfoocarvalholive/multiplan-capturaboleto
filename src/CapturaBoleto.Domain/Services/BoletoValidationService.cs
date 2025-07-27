using CapturaBoleto.Domain.Entities;
using CapturaBoleto.Domain.ValueObjects;

namespace CapturaBoleto.Domain.Services;

/// <summary>
/// Serviço de domínio para validação de boletos
/// </summary>
public class BoletoValidationService
{
    public static ValidationResult ValidarBoleto(Boleto boleto)
    {
        var erros = new List<string>();

        // Validar data de vencimento
        if (boleto.DataVencimento < DateTime.Now.AddYears(-10))
        {
            erros.Add("Data de vencimento muito antiga (mais de 10 anos)");
        }

        if (boleto.DataVencimento > DateTime.Now.AddYears(5))
        {
            erros.Add("Data de vencimento muito distante (mais de 5 anos)");
        }

        // Validar valor
        if (boleto.Valor > 999999.99m)
        {
            erros.Add("Valor do boleto excede o limite máximo (R$ 999.999,99)");
        }

        // Validar beneficiário
        if (boleto.Beneficiario.Length < 3)
        {
            erros.Add("Nome do beneficiário muito curto");
        }

        // Validar código de barras
        if (!ValidarDigitoVerificadorCodigoBarras(boleto.CodigoBarras.Valor))
        {
            erros.Add("Dígito verificador do código de barras inválido");
        }

        return new ValidationResult
        {
            IsValid = !erros.Any(),
            Errors = erros
        };
    }

    private static bool ValidarDigitoVerificadorCodigoBarras(string codigoBarras)
    {
        if (codigoBarras.Length != 44)
            return false;

        var digitos = codigoBarras.Select(c => int.Parse(c.ToString())).ToArray();
        var digitoVerificador = digitos[4];

        // Remove o dígito verificador para calcular
        var codigoSemDV = codigoBarras[..4] + codigoBarras[5..];
        
        var sequencia = new[] { 2, 3, 4, 5, 6, 7, 8, 9 };
        var soma = 0;
        var sequenciaIndex = 0;

        for (int i = codigoSemDV.Length - 1; i >= 0; i--)
        {
            soma += int.Parse(codigoSemDV[i].ToString()) * sequencia[sequenciaIndex % 8];
            sequenciaIndex++;
        }

        var resto = soma % 11;
        var dvCalculado = resto == 0 || resto == 10 || resto == 11 ? 1 : 11 - resto;

        return digitoVerificador == dvCalculado;
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}