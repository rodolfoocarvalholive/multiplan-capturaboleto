# CapturaBoleto API 📊

API para captura e processamento de boletos bancários desenvolvida em .NET 9 seguindo os princípios do Domain Driven Design (DDD) com **logging estruturado Serilog** e **arquitetura modular**.

## 🏗️ Arquitetura

A solução está organizada em 4 camadas principais seguindo o padrão DDD com **configurações modularizadas**:

### 📦 Camadas

- **CapturaBoleto.Domain**: Núcleo da aplicação contendo entidades, value objects, repositories e serviços de domínio
- **CapturaBoleto.Infrastructure**: Implementações técnicas (repositórios, contexto de dados, serviços externos)
- **CapturaBoleto.Application**: Casos de uso, DTOs e serviços de aplicação
- **CapturaBoleto.WebAPI**: Interface HTTP RESTful com documentação Swagger

### 🎯 Estrutura de Pastas

```
src/
├── CapturaBoleto.Domain/
│   ├── Entities/          # Entidades do domínio
│   ├── ValueObjects/      # Objetos de valor
│   ├── Repositories/      # Interfaces de repositório
│   └── Services/          # Serviços de domínio
├── CapturaBoleto.Infrastructure/
│   ├── Data/              # Contexto e configurações de dados
│   └── Repositories/      # Implementações dos repositórios
├── CapturaBoleto.Application/
│   ├── DTOs/              # Data Transfer Objects
│   └── Services/          # Serviços de aplicação e logging
└── CapturaBoleto.WebAPI/
    ├── Controllers/       # Controllers da API
    ├── Configuration/     # 🆕 Classes de configuração modular
    │   ├── SerilogConfiguration.cs
    │   ├── SwaggerConfiguration.cs
    │   ├── DependencyInjectionConfiguration.cs
    │   ├── MiddlewareConfiguration.cs
    │   └── README.md
    └── Program.cs         # ✨ Program.cs limpo e organizado
```

## 🚀 Tecnologias

- **.NET 9**: Framework principal
- **ASP.NET Core**: Web API
- **Swashbuckle**: Documentação Swagger/OpenAPI
- **Serilog**: Logging estruturado avançado
- **Entity Framework Core**: ORM (a ser implementado)

## ✨ **Nova Arquitetura Modular - Program.cs Organizado**

### 🎯 **Program.cs Limpo e Focado:**

```csharp
try
{
    Log.Information("Iniciando CapturaBoleto API");

    var builder = WebApplication.CreateBuilder(args);

    // ===== CONFIGURAÇÃO DE LOGGING =====
    builder.ConfigureSerilog();

    // ===== CONFIGURAÇÃO DE SERVIÇOS =====
    builder.Services
        .AddApplicationServices(builder.Configuration)
        .AddSwaggerConfiguration()
        .AddCorsConfiguration()
        .AddCacheConfiguration()
        .AddCompressionConfiguration();

    // ===== CONSTRUÇÃO DA APLICAÇÃO =====
    var app = builder.Build();

    // ===== CONFIGURAÇÃO DO PIPELINE DE MIDDLEWARE =====
    app.ConfigureMiddlewarePipeline();

    Log.Information("CapturaBoleto API configurada com sucesso");
    
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

### 🏗️ **Classes de Configuração Especializadas:**

#### **SerilogConfiguration.cs**
- 📊 Configuração completa do Serilog
- 🔧 Early logging para inicialização
- 📝 Middleware de logging HTTP
- 🎛️ Enrichers e formatação personalizada

#### **SwaggerConfiguration.cs**
- 📚 Documentação OpenAPI avançada
- 🎨 Configuração personalizada do Swagger UI
- 📖 Inclusão automática de comentários XML
- 🔐 Preparado para autenticação

#### **DependencyInjectionConfiguration.cs**
- 🎯 Registro modular por camadas DDD
- 🌐 Configuração de CORS
- ⚡ Cache em memória
- 📦 Compressão de resposta

#### **MiddlewareConfiguration.cs**
- 🛡️ Middleware de segurança
- 🔧 Pipeline diferenciado por ambiente
- ⚠️ Tratamento global de exceções
- 🔒 Headers de segurança

## 📊 Sistema de Logging Avançado

### 🎯 **Serilog - Logging Estruturado**

A aplicação utiliza **Serilog** como sistema de logging estruturado com as seguintes características:

#### ✨ **Características Implementadas:**

- **Logging Estruturado**: Logs em formato JSON com propriedades tipadas
- **Múltiplos Sinks**: Console, File, Debug
- **Enrichers**: Informações de contexto (máquina, ambiente, processo, thread)
- **Níveis Configuráveis**: Por namespace e ambiente
- **Rotação de Arquivos**: Logs diários com retenção configurável
- **Request Logging**: Middleware automático para requisições HTTP
- **Performance Metrics**: Métricas de tempo de execução

#### 📁 **Estrutura de Logs:**

```
logs/
├── capturaboleto-20250127.log          # Logs gerais (diários)
├── capturaboleto-errors-20250127.log   # Apenas erros e warnings
└── production/                         # Logs de produção (separados)
    ├── capturaboleto-20250127.log
    └── capturaboleto-errors-20250127.log
```

## 📋 Funcionalidades

### ✅ Implementadas

- [x] **🏗️ Arquitetura Modular com Classes de Configuração**
- [x] **✨ Program.cs Limpo e Organizado**
- [x] **🔧 Configurações Especializadas por Responsabilidade**
- [x] Estrutura DDD completa
- [x] **Sistema de Logging Estruturado (Serilog)**
- [x] **Múltiplos Sinks de Log (Console, File, Debug)**
- [x] **Logging de Requisições HTTP Automático**
- [x] **Métricas de Performance**
- [x] **Logs Específicos do Domínio**
- [x] **Middleware de Segurança Avançado**
- [x] **Tratamento Global de Exceções**
- [x] Entidades de domínio (Boleto)
- [x] Value Objects (CodigoBarras, Documento)
- [x] Validações de negócio
- [x] API RESTful com Swagger
- [x] Documentação detalhada da API
- [x] Validação de CPF/CNPJ
- [x] Validação de código de barras
- [x] Conversão para linha digitável

### 🔄 Em Desenvolvimento

- [ ] Implementação dos repositórios
- [ ] Configuração do Entity Framework
- [ ] Testes unitários
- [ ] Testes de integração
- [ ] Autenticação e autorização
- [ ] Dashboard de métricas de logs
- [ ] Health checks
- [ ] Rate limiting

## 🏃‍♂️ Como Executar

### Pré-requisitos

- .NET 9 SDK
- Visual Studio 2022 ou VS Code

### Executando a API

```bash
# Clone o repositório
git clone <url-do-repositorio>
cd capturaboleto

# Restaurar dependências
dotnet restore

# Executar a API
dotnet run --project src/CapturaBoleto.WebAPI
```

A API estará disponível em:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001` (raiz da aplicação)

**Logs serão gerados automaticamente em:**
- **Console**: Output colorido durante desenvolvimento
- **Arquivo**: `logs/capturaboleto-YYYYMMDD.log`
- **Erros**: `logs/capturaboleto-errors-YYYYMMDD.log`

## 🎯 **Benefícios da Nova Arquitetura**

### ✅