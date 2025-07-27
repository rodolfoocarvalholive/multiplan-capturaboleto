using Microsoft.AspNetCore.Mvc;
using CapturaBoleto.Application.DTOs;
using CapturaBoleto.Application.Services;
using CapturaBoleto.Domain.Entities;
using CapturaBoleto.Domain.ValueObjects;
using CapturaBoleto.Domain.Services;
using System.Diagnostics;

namespace CapturaBoleto.WebAPI.Controllers;

/// <summary>
/// Controller para operações com boletos bancários
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BoletosController : ControllerBase
{
    private readonly ILogger<BoletosController> _logger;
    private readonly BoletoLoggingService _boletoLogger;

    public BoletosController(
        ILogger<BoletosController> logger,
        BoletoLoggingService boletoLogger)
    {
        _logger = logger;
        _boletoLogger = boletoLogger;
    }

    /// <summary>
    /// Cria um novo boleto no sistema
    /// </summary>
    /// <param name="criarBoletoDto">Dados do boleto a ser criado</param>
    /// <returns>Boleto criado com sucesso</returns>
    /// <response code="201">Boleto criado com sucesso</response>
    /// <response code="400">Dados inválidos fornecidos</response>
    /// <response code="409">Boleto com este código de barras já existe</response>
    [HttpPost]
    [ProducesResponseType(typeof(BoletoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BoletoDto>> CriarBoleto([FromBody] CriarBoletoDto criarBoletoDto)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _boletoLogger.LogApiRequestStart(
            "POST /api/boletos", 
            HttpContext.Request.Method, 
            HttpContext.User.Identity?.Name);

        try
        {
            _logger.LogInformation("Iniciando criação de boleto com código de barras: {CodigoBarras}", 
                criarBoletoDto.CodigoBarras);

            // Criar objetos de domínio
            var codigoBarras = new CodigoBarras(criarBoletoDto.CodigoBarras);
            
            Documento? documentoBeneficiario = null;
            if (!string.IsNullOrWhiteSpace(criarBoletoDto.DocumentoBeneficiario))
            {
                documentoBeneficiario = new Documento(criarBoletoDto.DocumentoBeneficiario);
            }

            var boleto = new Boleto(
                codigoBarras,
                criarBoletoDto.Valor,
                criarBoletoDto.DataVencimento,
                criarBoletoDto.Beneficiario,
                documentoBeneficiario);

            // Configurar pagador se fornecido
            if (!string.IsNullOrWhiteSpace(criarBoletoDto.Pagador))
            {
                Documento? documentoPagador = null;
                if (!string.IsNullOrWhiteSpace(criarBoletoDto.DocumentoPagador))
                {
                    documentoPagador = new Documento(criarBoletoDto.DocumentoPagador);
                }
                boleto.DefinirPagador(criarBoletoDto.Pagador, documentoPagador);
            }

            // Configurar identificadores
            boleto.DefinirIdentificadores(criarBoletoDto.NossoNumero, criarBoletoDto.NumeroDocumento);

            // Validar boleto
            var validationResult = BoletoValidationService.ValidarBoleto(boleto);
            if (!validationResult.IsValid)
            {
                _boletoLogger.LogBoletoValidationError(criarBoletoDto.CodigoBarras, validationResult.Errors);
                
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                stopwatch.Stop();
                _boletoLogger.LogApiRequestEnd(
                    "POST /api/boletos", 
                    HttpContext.Request.Method, 
                    400, 
                    stopwatch.Elapsed);

                return ValidationProblem(ModelState);
            }

            // TODO: Implementar salvamento no repositório
            // var existingBoleto = await _boletoRepository.ObterPorCodigoBarrasAsync(codigoBarras.Valor);
            // if (existingBoleto != null)
            // {
            //     _boletoLogger.LogDuplicateBoleto(codigoBarras.Valor);
            //     return Conflict("Boleto com este código de barras já existe");
            // }
            // await _boletoRepository.AdicionarAsync(boleto);

            _boletoLogger.LogBoletoCreated(
                boleto.Id, 
                boleto.CodigoBarras.Valor, 
                boleto.Valor, 
                boleto.Beneficiario);

            var boletoDto = MapearParaDto(boleto);

            stopwatch.Stop();
            _boletoLogger.LogPerformanceMetric("CriarBoleto", stopwatch.Elapsed);
            _boletoLogger.LogApiRequestEnd(
                "POST /api/boletos", 
                HttpContext.Request.Method, 
                201, 
                stopwatch.Elapsed);

            return CreatedAtAction(
                nameof(ObterBoleto), 
                new { id = boleto.Id }, 
                boletoDto);
        }
        catch (ArgumentException ex)
        {
            stopwatch.Stop();
            
            _logger.LogWarning(ex, "Dados inválidos ao criar boleto: {Message}", ex.Message);
            _boletoLogger.LogApiRequestEnd(
                "POST /api/boletos", 
                HttpContext.Request.Method, 
                400, 
                stopwatch.Elapsed);

            ModelState.AddModelError("", ex.Message);
            return ValidationProblem(ModelState);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex, "Erro interno ao criar boleto");
            _boletoLogger.LogRepositoryError("CriarBoleto", null, ex);
            _boletoLogger.LogApiRequestEnd(
                "POST /api/boletos", 
                HttpContext.Request.Method, 
                500, 
                stopwatch.Elapsed);

            return Problem("Erro interno do servidor", statusCode: 500);
        }
    }

    /// <summary>
    /// Obtém um boleto pelo ID
    /// </summary>
    /// <param name="id">ID do boleto</param>
    /// <returns>Dados do boleto</returns>
    /// <response code="200">Boleto encontrado</response>
    /// <response code="404">Boleto não encontrado</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BoletoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BoletoDto>> ObterBoleto(Guid id)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _boletoLogger.LogApiRequestStart(
            $"GET /api/boletos/{id}", 
            HttpContext.Request.Method, 
            HttpContext.User.Identity?.Name);

        try
        {
            _logger.LogInformation("Buscando boleto com ID: {BoletoId}", id);

            // TODO: Implementar busca no repositório
            // var boleto = await _boletoRepository.ObterPorIdAsync(id);
            // if (boleto == null)
            // {
            //     _boletoLogger.LogBoletoNotFound(id);
            //     return NotFound($"Boleto com ID {id} não encontrado");
            // }

            // Simulação temporária
            _boletoLogger.LogBoletoNotFound(id);
            
            stopwatch.Stop();
            _boletoLogger.LogApiRequestEnd(
                $"GET /api/boletos/{id}", 
                HttpContext.Request.Method, 
                404, 
                stopwatch.Elapsed);

            return NotFound($"Boleto com ID {id} não encontrado");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex, "Erro interno ao buscar boleto {BoletoId}", id);
            _boletoLogger.LogRepositoryError("ObterBoleto", id, ex);
            _boletoLogger.LogApiRequestEnd(
                $"GET /api/boletos/{id}", 
                HttpContext.Request.Method, 
                500, 
                stopwatch.Elapsed);

            return Problem("Erro interno do servidor", statusCode: 500);
        }
    }

    /// <summary>
    /// Lista boletos por status
    /// </summary>
    /// <param name="status">Status dos boletos (Pendente, Processado, Rejeitado)</param>
    /// <returns>Lista de boletos</returns>
    /// <response code="200">Lista de boletos</response>
    /// <response code="400">Status inválido</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BoletoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<BoletoDto>>> ListarBoletos([FromQuery] string? status = null)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _boletoLogger.LogApiRequestStart(
            "GET /api/boletos", 
            HttpContext.Request.Method, 
            HttpContext.User.Identity?.Name);

        try
        {
            _logger.LogInformation("Listando boletos. Filtro de status: {Status}", status ?? "Todos");

            if (!string.IsNullOrEmpty(status))
            {
                if (!Enum.TryParse<StatusBoleto>(status, true, out var statusEnum))
                {
                    _boletoLogger.LogBusinessRuleViolation(
                        "StatusInvalido", 
                        $"Status fornecido '{status}' não é válido");

                    ModelState.AddModelError(nameof(status), "Status inválido. Valores aceitos: Pendente, Processado, Rejeitado");
                    
                    stopwatch.Stop();
                    _boletoLogger.LogApiRequestEnd(
                        "GET /api/boletos", 
                        HttpContext.Request.Method, 
                        400, 
                        stopwatch.Elapsed);

                    return ValidationProblem(ModelState);
                }
            }

            // TODO: Implementar busca no repositório
            // var boletos = string.IsNullOrEmpty(status) 
            //     ? await _boletoRepository.ObterTodosAsync()
            //     : await _boletoRepository.ObterPorStatusAsync(statusEnum);

            // Simulação temporária
            var boletos = new List<BoletoDto>();

            stopwatch.Stop();
            _boletoLogger.LogPerformanceMetric("ListarBoletos", stopwatch.Elapsed, boletos.Count());
            _boletoLogger.LogApiRequestEnd(
                "GET /api/boletos", 
                HttpContext.Request.Method, 
                200, 
                stopwatch.Elapsed);

            return Ok(boletos);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex, "Erro interno ao listar boletos");
            _boletoLogger.LogRepositoryError("ListarBoletos", null, ex);
            _boletoLogger.LogApiRequestEnd(
                "GET /api/boletos", 
                HttpContext.Request.Method, 
                500, 
                stopwatch.Elapsed);

            return Problem("Erro interno do servidor", statusCode: 500);
        }
    }

    /// <summary>
    /// Processa um boleto (marca como processado)
    /// </summary>
    /// <param name="id">ID do boleto</param>
    /// <param name="observacoes">Observações do processamento</param>
    /// <returns>Boleto processado</returns>
    /// <response code="200">Boleto processado com sucesso</response>
    /// <response code="404">Boleto não encontrado</response>
    [HttpPatch("{id:guid}/processar")]
    [ProducesResponseType(typeof(BoletoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BoletoDto>> ProcessarBoleto(Guid id, [FromBody] string? observacoes = null)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _boletoLogger.LogApiRequestStart(
            $"PATCH /api/boletos/{id}/processar", 
            HttpContext.Request.Method, 
            HttpContext.User.Identity?.Name);

        try
        {
            _logger.LogInformation("Processando boleto {BoletoId}", id);

            // TODO: Implementar busca e atualização no repositório
            // var boleto = await _boletoRepository.ObterPorIdAsync(id);
            // if (boleto == null)
            // {
            //     _boletoLogger.LogBoletoNotFound(id);
            //     return NotFound($"Boleto com ID {id} não encontrado");
            // }
            // 
            // boleto.MarcarComoProcessado(observacoes);
            // await _boletoRepository.AtualizarAsync(boleto);
            // 
            // _boletoLogger.LogBoletoProcessed(id, observacoes);

            _boletoLogger.LogBoletoNotFound(id);

            stopwatch.Stop();
            _boletoLogger.LogApiRequestEnd(
                $"PATCH /api/boletos/{id}/processar", 
                HttpContext.Request.Method, 
                404, 
                stopwatch.Elapsed);

            return NotFound($"Boleto com ID {id} não encontrado");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex, "Erro interno ao processar boleto {BoletoId}", id);
            _boletoLogger.LogRepositoryError("ProcessarBoleto", id, ex);
            _boletoLogger.LogApiRequestEnd(
                $"PATCH /api/boletos/{id}/processar", 
                HttpContext.Request.Method, 
                500, 
                stopwatch.Elapsed);

            return Problem("Erro interno do servidor", statusCode: 500);
        }
    }

    private static BoletoDto MapearParaDto(Boleto boleto)
    {
        return new BoletoDto
        {
            Id = boleto.Id,
            CodigoBarras = boleto.CodigoBarras.Valor,
            LinhaDigitavel = boleto.LinhaDigitavel,
            Valor = boleto.Valor,
            DataVencimento = boleto.DataVencimento,
            Beneficiario = boleto.Beneficiario,
            DocumentoBeneficiario = boleto.DocumentoBeneficiario?.ToString(),
            Pagador = boleto.Pagador,
            DocumentoPagador = boleto.DocumentoPagador?.ToString(),
            NossoNumero = boleto.NossoNumero,
            NumeroDocumento = boleto.NumeroDocumento,
            Status = boleto.Status.ToString(),
            DataCriacao = boleto.DataCriacao,
            DataProcessamento = boleto.DataProcessamento,
            ObservacoesProcessamento = boleto.ObservacoesProcessamento,
            EstaVencido = boleto.EstaVencido(),
            DiasParaVencimento = boleto.DiasParaVencimento()
        };
    }
}