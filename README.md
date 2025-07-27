# CapturaBoleto API ??

API para captura e processamento de boletos bancários desenvolvida em .NET 9 seguindo os princípios do Domain Driven Design (DDD) com **logging estruturado Serilog**.

## ??? Arquitetura

A solução está organizada em 4 camadas principais seguindo o padrão DDD:

### ?? Camadas

- **CapturaBoleto.Domain**: Núcleo da aplicação contendo entidades, value objects, repositories e serviços de domínio
- **CapturaBoleto.Infrastructure**: Implementações técnicas (repositórios, contexto de dados, serviços externos)
- **CapturaBoleto.Application**: Casos de uso, DTOs e serviços de aplicação
- **CapturaBoleto.WebAPI**: Interface HTTP RESTful com documentação Swagger

### ?? Estrutura de Pastas

```
src/
??? CapturaBoleto.Domain/
?   ??? Entities/          # Entidades do domínio
?   ??? ValueObjects/      # Objetos de valor
?   ??? Repositories/      # Interfaces de repositório
?   ??? Services/          # Serviços de domínio
??? CapturaBoleto.Infrastructure/
?   ??? Data/              # Contexto e configurações de dados
?   ??? Repositories/      # Implementações dos repositórios
??? CapturaBoleto.Application/
?   ??? DTOs/              # Data Transfer Objects
?   ??? Services/          # Serviços de aplicação e logging
??? CapturaBoleto.WebAPI/
    ??? Controllers/       # Controllers da API
    ??? Program.cs         # Configuração da aplicação
```

## ?? Tecnologias

- **.NET 9**: Framework principal
- **ASP.NET Core**: Web API
- **Swashbuckle**: Documentação Swagger/OpenAPI
- **Serilog**: Logging estruturado avançado
- **Entity Framework Core**: ORM (a ser implementado)

## ?? Sistema de Logging Avançado

### ?? **Serilog - Logging Estruturado**

A aplicação utiliza **Serilog** como sistema de logging estruturado com as seguintes características:

#### ? **Características Implementadas:**

- **Logging Estruturado**: Logs em formato JSON com propriedades tipadas
- **Múltiplos Sinks**: Console, File, Debug
- **Enrichers**: Informações de contexto (máquina, ambiente, processo, thread)
- **Níveis Configuráveis**: Por namespace e ambiente
- **Rotação de Arquivos**: Logs diários com retenção configurável
- **Request Logging**: Middleware automático para requisições HTTP
- **Performance Metrics**: Métricas de tempo de execução

#### ?? **Estrutura de Logs:**

```
logs/
??? capturaboleto-20250127.log          # Logs gerais (diários)
??? capturaboleto-errors-20250127.log   # Apenas erros e warnings
??? production/                         # Logs de produção (separados)
    ??? capturaboleto-20250127.log
    ??? capturaboleto-errors-20250127.log
```

#### ??? **Configuração por Ambiente:**

- **Development**: Logs DEBUG, console colorido, arquivos locais
- **Production**: Logs INFO+, otimizados, retenção estendida

#### ?? **Tipos de Logs Implementados:**

1. **Logs de Domínio** (`BoletoLoggingService`):
   - Criação de boletos
   - Validações de negócio
   - Processamento e rejeições
   - Métricas de performance

2. **Logs de API**:
   - Requisições HTTP (início/fim)
   - Códigos de status
   - Tempo de resposta
   - Headers e contexto

3. **Logs de Sistema**:
   - Inicialização da aplicação
   - Erros de repositório
   - Violações de regras de negócio

### ?? **Exemplo de Configuração:**

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "CapturaBoleto": "Debug"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/capturaboleto-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ]
  }
}
```

## ?? Funcionalidades

### ? Implementadas

- [x] Estrutura DDD completa
- [x] **Sistema de Logging Estruturado (Serilog)**
- [x] **Múltiplos Sinks de Log (Console, File, Debug)**
- [x] **Logging de Requisições HTTP Automático**
- [x] **Métricas de Performance**
- [x] **Logs Específicos do Domínio**
- [x] Entidades de domínio (Boleto)
- [x] Value Objects (CodigoBarras, Documento)
- [x] Validações de negócio
- [x] API RESTful com Swagger
- [x] Documentação detalhada da API
- [x] Validação de CPF/CNPJ
- [x] Validação de código de barras
- [x] Conversão para linha digitável

### ?? Em Desenvolvimento

- [ ] Implementação dos repositórios
- [ ] Configuração do Entity Framework
- [ ] Testes unitários
- [ ] Testes de integração
- [ ] Autenticação e autorização
- [ ] Dashboard de métricas de logs

## ????? Como Executar

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

## ?? Documentação da API

### Endpoints Principais

#### POST /api/boletos
Cria um novo boleto no sistema.

**Request Body:**
```json
{
  "codigoBarras": "12345678901234567890123456789012345678901234",
  "valor": 150.75,
  "dataVencimento": "2024-12-31T00:00:00",
  "beneficiario": "Empresa ABC Ltda",
  "documentoBeneficiario": "12.345.678/0001-90",
  "pagador": "João Silva",
  "documentoPagador": "123.456.789-00",
  "nossoNumero": "0001234567",
  "numeroDocumento": "DOC-001"
}
```

**Logs Gerados:**
```json
{
  "Timestamp": "2025-01-27T17:25:30.123Z",
  "Level": "Information",
  "Message": "Boleto criado com sucesso",
  "Properties": {
    "BoletoId": "123e4567-e89b-12d3-a456-426614174000",
    "CodigoBarras": "12345678901234567890123456789012345678901234",
    "Valor": 150.75,
    "Beneficiario": "Empresa ABC Ltda"
  }
}
```

#### GET /api/boletos/{id}
Obtém um boleto pelo ID.

#### GET /api/boletos?status=Pendente
Lista boletos filtrados por status.

#### PATCH /api/boletos/{id}/processar
Marca um boleto como processado.

## ?? Logs e Monitoramento

### ?? **Métricas Disponíveis:**

- **Tempo de resposta** de cada endpoint
- **Taxa de erros** por tipo
- **Volume de requisições** por hora/dia
- **Performance** de operações do domínio
- **Validações rejeitadas** e motivos

### ?? **Exemplo de Log Estruturado:**

```json
{
  "Timestamp": "2025-01-27T17:25:30.123-03:00",
  "Level": "Information",
  "MessageTemplate": "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms",
  "Message": "HTTP POST /api/boletos responded 201 in 45.2341 ms",
  "Properties": {
    "RequestMethod": "POST",
    "RequestPath": "/api/boletos",
    "StatusCode": 201,
    "Elapsed": 45.2341,
    "RequestHost": "localhost:5001",
    "UserAgent": "Mozilla/5.0...",
    "RemoteIP": "127.0.0.1",
    "MachineName": "DEV-MACHINE",
    "EnvironmentName": "Development",
    "ProcessId": 1234,
    "ThreadId": 5
  }
}
```

## ?? Exemplos de Uso

### Criar um boleto válido

```bash
curl -X POST "https://localhost:5001/api/boletos" \
  -H "Content-Type: application/json" \
  -d '{
    "codigoBarras": "12345678901234567890123456789012345678901234",
    "valor": 150.75,
    "dataVencimento": "2024-12-31T00:00:00",
    "beneficiario": "Empresa ABC Ltda",
    "documentoBeneficiario": "12345678000190"
  }'
```

### Monitorar logs em tempo real

```bash
# Acompanhar logs gerais
tail -f logs/capturaboleto-$(date +%Y%m%d).log

# Acompanhar apenas erros
tail -f logs/capturaboleto-errors-$(date +%Y%m%d).log
```

## ?? Configuração

### appsettings.json (Desenvolvimento)

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "CapturaBoleto": "Debug"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/capturaboleto-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithEnvironmentName" ]
  }
}
```

## ?? Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## ?? Licença

Este projeto está sob licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## ?? Equipe

- **Desenvolvimento**: Equipe DG Multiplan
- **Arquitetura**: Domain Driven Design
- **Framework**: .NET 9
- **Logging**: Serilog (Structured Logging)