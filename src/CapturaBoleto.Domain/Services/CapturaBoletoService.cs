using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapturaBoleto.Domain.Entities;
using CapturaBoleto.Domain.Repositories;
using CapturaBoleto.Domain.ValueObjects;

namespace CapturaBoleto.Domain.Services
{
    /// <summary>
    /// Serviço de domínio para coordenar operações de captura de boletos
    /// </summary>
    public class CapturaBoletoService(IBoletoExternalService boletoExternalService, IBoletoRepository boletoRepository)
    {
        private readonly IBoletoRepository _boletoRepository = boletoRepository;
        private readonly IBoletoExternalService _boletoExternalService = boletoExternalService;

        /// <summary>
        /// Inicia uma nova sessão de captura de boleto com autenticação
        /// </summary>
        /// <param name="usuario">Nome de usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <returns>Resultado da autenticação com token de sessão</returns>
        public async Task<ResultadoAutenticacao> IniciarCapturaBoletoAsync(string usuario, string senha)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("Usuário não pode ser nulo ou vazio", nameof(usuario));

            if (string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("Senha não pode ser nula ou vazia", nameof(senha));

            // Validar credenciais (implementação básica para demonstração)
            var credenciaisValidas = await ValidarCredenciaisAsync(usuario, senha);
            
            if (!credenciaisValidas)
            {
                return new ResultadoAutenticacao
                {
                    Sucesso = false,
                    MensagemErro = "Usuário ou senha inválidos",
                    DataTentativa = DateTime.UtcNow
                };
            }

            // Gerar token de sessão
            var tokenSessao = GerarTokenSessao(usuario);
            
            return new ResultadoAutenticacao
            {
                Sucesso = true,
                Usuario = usuario,
                TokenSessao = tokenSessao,
                DataAutenticacao = DateTime.UtcNow,
                DataExpiracaoToken = DateTime.UtcNow.AddHours(8) // Token válido por 8 horas
            };
        }

        /// <summary>
        /// Inicia o processo de captura de um boleto específico
        /// </summary>
        /// <param name="tokenSessao">Token de autenticação da sessão</param>
        /// <param name="codigoBarras">Código de barras do boleto</param>
        /// <returns>Informações iniciais do boleto para captura</returns>
        public async Task<ResultadoInicioCaptura> IniciarCapturaAsync(string tokenSessao, string codigoBarras)
        {
            if (string.IsNullOrWhiteSpace(tokenSessao))
                throw new ArgumentException("Token de sessão não pode ser nulo ou vazio", nameof(tokenSessao));

            if (string.IsNullOrWhiteSpace(codigoBarras))
                throw new ArgumentException("Código de barras não pode ser nulo ou vazio", nameof(codigoBarras));

            // Validar token de sessão
            var tokenValido = await ValidarTokenSessaoAsync(tokenSessao);
            if (!tokenValido)
            {
                return new ResultadoInicioCaptura
                {
                    Sucesso = false,
                    MensagemErro = "Token de sessão inválido ou expirado"
                };
            }

            try
            {
                // Validar formato do código de barras
                var codigoBarrasObj = new CodigoBarras(codigoBarras);
                
                // Verificar se o boleto já existe no sistema
                var boletoExistente = await _boletoRepository.ObterPorCodigoBarrasAsync(codigoBarras);
                if (boletoExistente != null)
                {
                    return new ResultadoInicioCaptura
                    {
                        Sucesso = false,
                        MensagemErro = "Boleto já existe no sistema",
                        BoletoExistente = boletoExistente
                    };
                }

                // Extrair informações básicas do código de barras
                var informacoesBasicas = ExtrairInformacoesDoCodigoBarras(codigoBarrasObj);

                return new ResultadoInicioCaptura
                {
                    Sucesso = true,
                    CodigoBarras = codigoBarras,
                    LinhaDigitavel = codigoBarrasObj.GetLinhaDigitavel(),
                    InformacoesBasicas = informacoesBasicas,
                    SessaoId = Guid.NewGuid().ToString()
                };
            }
            catch (ArgumentException ex)
            {
                return new ResultadoInicioCaptura
                {
                    Sucesso = false,
                    MensagemErro = $"Código de barras inválido: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Valida as credenciais do usuário
        /// </summary>
        private async Task<bool> ValidarCredenciaisAsync(string usuario, string senha)
        {
            // TODO: Implementar validação real contra base de dados ou serviço externo
            // Por enquanto, implementação básica para demonstração
            await Task.Delay(100); // Simular operação assíncrona

            // Credenciais básicas para demonstração - em produção, usar hash da senha
            var usuariosValidos = new Dictionary<string, string>
            {
                { "admin", "123456" },
                { "operador", "boleto2024" },
                { "supervisor", "multiplan123" }
            };

            return usuariosValidos.ContainsKey(usuario.ToLower()) && 
                   usuariosValidos[usuario.ToLower()] == senha;
        }

        /// <summary>
        /// Gera um token de sessão para o usuário autenticado
        /// </summary>
        private static string GerarTokenSessao(string usuario)
        {
            // TODO: Implementar geração de JWT ou token mais seguro
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var tokenData = $"{usuario}:{timestamp}:{Guid.NewGuid()}";
            
            // Em produção, usar criptografia adequada
            var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(tokenData));
            return token;
        }

        /// <summary>
        /// Valida se o token de sessão é válido e não expirou
        /// </summary>
        private async Task<bool> ValidarTokenSessaoAsync(string token)
        {
            // TODO: Implementar validação real do token
            await Task.Delay(50); // Simular operação assíncrona
            
            try
            {
                var tokenData = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var parts = tokenData.Split(':');
                
                if (parts.Length != 3) return false;
                
                var timestamp = long.Parse(parts[1]);
                var tokenTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                var agora = DateTimeOffset.UtcNow;
                
                // Token válido por 8 horas
                return agora.Subtract(tokenTime).TotalHours < 8;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Extrai informações básicas do código de barras
        /// </summary>
        private static InformacoesBoletoBasicas ExtrairInformacoesDoCodigoBarras(CodigoBarras codigoBarras)
        {
            var codigo = codigoBarras.Valor;
            
            // Extrair informações conforme padrão FEBRABAN
            var bancoCodigo = codigo[0..3];
            var moedaCodigo = codigo[3].ToString();
            var digitoVerificador = codigo[4].ToString();
            var fatorVencimento = codigo[5..9];
            var valor = codigo[9..19];

            return new InformacoesBoletoBasicas
            {
                CodigoBanco = bancoCodigo,
                CodigoMoeda = moedaCodigo,
                DigitoVerificador = digitoVerificador,
                FatorVencimento = fatorVencimento,
                ValorNominal = valor,
                DataVencimentoCalculada = CalcularDataVencimento(fatorVencimento)
            };
        }

        /// <summary>
        /// Calcula a data de vencimento baseada no fator de vencimento
        /// </summary>
        private static DateTime? CalcularDataVencimento(string fatorVencimento)
        {
            if (!int.TryParse(fatorVencimento, out int fator) || fator == 0)
                return null;

            // Data base: 07/10/1997
            var dataBase = new DateTime(1997, 10, 7);
            return dataBase.AddDays(fator);
        }
    }

    /// <summary>
    /// Resultado da operação de autenticação
    /// </summary>
    public class ResultadoAutenticacao
    {
        public bool Sucesso { get; set; }
        public string? Usuario { get; set; }
        public string? TokenSessao { get; set; }
        public DateTime? DataAutenticacao { get; set; }
        public DateTime? DataExpiracaoToken { get; set; }
        public string? MensagemErro { get; set; }
        public DateTime DataTentativa { get; set; }
    }

    /// <summary>
    /// Resultado da operação de início de captura
    /// </summary>
    public class ResultadoInicioCaptura
    {
        public bool Sucesso { get; set; }
        public string? CodigoBarras { get; set; }
        public string? LinhaDigitavel { get; set; }
        public string? SessaoId { get; set; }
        public InformacoesBoletoBasicas? InformacoesBasicas { get; set; }
        public Boleto? BoletoExistente { get; set; }
        public string? MensagemErro { get; set; }
    }

    /// <summary>
    /// Informações básicas extraídas do código de barras
    /// </summary>
    public class InformacoesBoletoBasicas
    {
        public string CodigoBanco { get; set; } = string.Empty;
        public string CodigoMoeda { get; set; } = string.Empty;
        public string DigitoVerificador { get; set; } = string.Empty;
        public string FatorVencimento { get; set; } = string.Empty;
        public string ValorNominal { get; set; } = string.Empty;
        public DateTime? DataVencimentoCalculada { get; set; }
    }
}
