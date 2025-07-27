namespace CapturaBoleto.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um documento (CPF/CNPJ)
/// </summary>
public class Documento
{
    public string Numero { get; private set; }
    public TipoDocumento Tipo { get; private set; }

    public Documento(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número do documento não pode ser nulo ou vazio", nameof(numero));

        var numeroLimpo = LimparDocumento(numero);
        
        if (IsCpf(numeroLimpo))
        {
            if (!ValidarCpf(numeroLimpo))
                throw new ArgumentException("CPF inválido", nameof(numero));
            
            Tipo = TipoDocumento.CPF;
        }
        else if (IsCnpj(numeroLimpo))
        {
            if (!ValidarCnpj(numeroLimpo))
                throw new ArgumentException("CNPJ inválido", nameof(numero));
            
            Tipo = TipoDocumento.CNPJ;
        }
        else
        {
            throw new ArgumentException("Documento deve ser um CPF (11 dígitos) ou CNPJ (14 dígitos) válido", nameof(numero));
        }

        Numero = numeroLimpo;
    }

    private static string LimparDocumento(string documento)
    {
        return new string(documento.Where(char.IsDigit).ToArray());
    }

    private static bool IsCpf(string documento) => documento.Length == 11;
    private static bool IsCnpj(string documento) => documento.Length == 14;

    private static bool ValidarCpf(string cpf)
    {
        if (cpf.All(c => c == cpf[0])) return false;

        var multiplicador1 = new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCpf = cpf[..9];
        var soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        var resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        var digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        digito += resto.ToString();

        return cpf.EndsWith(digito);
    }

    private static bool ValidarCnpj(string cnpj)
    {
        if (cnpj.All(c => c == cnpj[0])) return false;

        var multiplicador1 = new int[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new int[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCnpj = cnpj[..12];
        var soma = 0;

        for (int i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

        var resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        var digito = resto.ToString();
        tempCnpj += digito;
        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        digito += resto.ToString();

        return cnpj.EndsWith(digito);
    }

    public string GetNumeroFormatado()
    {
        return Tipo == TipoDocumento.CPF 
            ? $"{Numero[..3]}.{Numero[3..6]}.{Numero[6..9]}-{Numero[9..]}"
            : $"{Numero[..2]}.{Numero[2..5]}.{Numero[5..8]}/{Numero[8..12]}-{Numero[12..]}";
    }

    public override bool Equals(object? obj)
    {
        return obj is Documento other && Numero == other.Numero;
    }

    public override int GetHashCode()
    {
        return Numero.GetHashCode();
    }

    public override string ToString()
    {
        return GetNumeroFormatado();
    }
}

public enum TipoDocumento
{
    CPF,
    CNPJ
}