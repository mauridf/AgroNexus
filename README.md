# 🌾 AgroNexus - Sistema de Gestão para Produtor Rural

## Sobre
Sistema completo de gestão agrícola para pequenos, médios e grandes produtores rurais.

## Stack
- .NET 10 + C# 14
- Entity Framework Core 10
- PostgreSQL 16+
- JWT Authentication
- Scalar API Docs
- DbUp Migrations

## Como Rodar Localmente

### Pré-requisitos
- .NET 10 SDK
- PostgreSQL 16+

### Setup
\`\`\`bash
# Restaurar pacotes
dotnet restore

# Configurar secrets (desenvolvimento)
dotnet user-secrets init --project src/AgroNexus.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=agronexus_db;Username=postgres;Password=sua_senha" --project src/AgroNexus.Api
dotnet user-secrets set "Jwt:SecretKey" "sua-chave-super-secreta-com-mais-de-512-bits" --project src/AgroNexus.Api

# Rodar migrations
dotnet run --project src/AgroNexus.Api --migrate

# Iniciar API
dotnet run --project src/AgroNexus.Api

# Acessar documentação
# http://localhost:5000/scalar/v1
\`\`\`

## Arquitetura
- **DDD** (Domain-Driven Design)
- **SOLID** Principles
- **CQRS** (em evolução)
- Modular Monolith

## Estrutura
\`\`\`
src/
├── AgroNexus.Domain/         # Entidades, Interfaces, Regras de Negócio
├── AgroNexus.Application/    # Services, DTOs, Casos de Uso
├── AgroNexus.Infrastructure/ # EF Core, Repositories, Auth
├── AgroNexus.Api/            # Minimal API Endpoints
└── AgroNexus.CrossCutting/   # Validators, Exceptions, Extensions
\`\`\`

## Licença
MIT