# Configurações da Aplicação

Esta pasta contém as classes de configuração modularizadas da CapturaBoleto API.

## ?? Estrutura das Configurações

### `SerilogConfiguration.cs`
**Responsabilidade**: Configuração completa do sistema de logging Serilog
- ? Early logging para captura de logs de inicialização
- ? Configuração de enrichers (máquina, ambiente, processo, thread)
- ? Múltiplos sinks (Console, File, Debug)
- ? Middleware de logging de requisições HTTP
- ? Formatação personalizada de logs

### `SwaggerConfiguration.cs`
**Responsabilidade**: Configuração da documentação da API
- ? Configuração do Swagger/OpenAPI
- ? Metadados da API (título, versão, descrição, contato)
- ? Inclusão de comentários XML
- ? Configurações avançadas do Swagger UI
- ? Preparado para autenticação futura

### `DependencyInjectionConfiguration.cs`
**Responsabilidade**: Registro de serviços de injeção de dependência
- ? Registro modular por camadas (Domain, Application, Infrastructure)
- ? Configuração de CORS
- ? Configuração de cache em memória
- ? Configuração de compressão de resposta
- ? Estrutura preparada para Entity Framework

### `MiddlewareConfiguration.cs`
**Responsabilidade**: Configuração do pipeline de middleware
- ? Pipeline diferenciado por ambiente (Development/Production)
- ? Middleware de segurança (headers de proteção)
- ? Tratamento global de exceções
- ? Configurações de HTTPS e CORS
- ? Middleware de compressão

## ?? Como Usar

### Program.cs Limpo e Organizado
```csharp
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configuração de logging
    builder.ConfigureSerilog();

    // Configuração de serviços
    builder.Services
        .AddApplicationServices(builder.Configuration)
        .AddSwaggerConfiguration()
        .AddCorsConfiguration()
        .AddCacheConfiguration()
        .AddCompressionConfiguration();

    var app = builder.Build();

    // Configuração do pipeline
    app.ConfigureMiddlewarePipeline();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro fatal ao inicializar a aplicação");
}
finally
{
    SerilogConfiguration.CloseSerilogLogger();
}
```

## ?? Benefícios da Modularização

### ? **Organização**
- Separação clara de responsabilidades
- Código mais legível e manutenível
- Program.cs limpo e focado no essencial

### ? **Extensibilidade**
- Fácil adição de novos serviços
- Configurações específicas por ambiente
- Preparado para crescimento da aplicação

### ? **Testabilidade**
- Classes de configuração podem ser testadas individualmente
- Fácil mock de configurações em testes

### ? **Reutilização**
- Configurações podem ser reutilizadas em outros projetos
- Padrões consistentes em toda a solução

## ?? Logs de Configuração

Cada classe de configuração gera logs estruturados:

```json
{
  "Timestamp": "2025-01-27T18:50:00.000Z",
  "Level": "Information",
  "Message": "Configurando Swagger/OpenAPI"
}
```

## ?? Próximos Passos

- [ ] Adicionar configuração de Entity Framework
- [ ] Implementar configuração de autenticação/autorização
- [ ] Adicionar configuração de health checks
- [ ] Configurar métricas e monitoramento
- [ ] Adicionar configuração de rate limiting