namespace CapturaBoleto.Domain.ValueObjects;

/// <summary>
/// Value Object que representa o código de barras de um boleto
/// </summary>
public class CodigoBarras
{
    public string Valor { get; private set; }

    public CodigoBarras(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Código de barras não pode ser nulo ou vazio", nameof(valor));

        if (valor.Length != 44)
            throw new ArgumentException("Código de barras deve ter 44 dígitos", nameof(valor));

        if (!valor.All(char.IsDigit))
            throw new ArgumentException("Código de barras deve conter apenas números", nameof(valor));

        Valor = valor;
    }

    public string GetLinhaDigitavel()
    {
        // Converte código de barras para linha digitável
        var campo1 = $"{Valor[0..4]}.{Valor[32..37]}";
        var campo2 = $"{Valor[37..42]}.{Valor[42..44]}";
        var campo3 = $"{Valor[4..14]}";
        var campo4 = Valor[4];
        var campo5 = Valor[5..19];

        return $"{campo1} {campo2} {campo3} {campo4} {campo5}";
    }

    public override bool Equals(object? obj)
    {
        return obj is CodigoBarras other && Valor == other.Valor;
    }

    public override int GetHashCode()
    {
        return Valor.GetHashCode();
    }

    public override string ToString()
    {
        return Valor;
    }
}