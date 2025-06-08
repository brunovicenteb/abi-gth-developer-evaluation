# 🛠️ Developer Evaluation Project - Sales API

API RESTful desenvolvida em .NET 8 seguindo os princípios de DDD e Clean Architecture. Esta aplicação realiza o gerenciamento de vendas, incluindo criação, atualização, cancelamento, e consulta com filtros dinâmicos, ordenação e paginação.

---

## ✅ Requisitos Atendidos

- [x] API CRUD para vendas (incluindo cancelamento)
- [x] Aplicação de regras de desconto por quantidade
- [x] Filtros dinâmicos por query string
- [x] Paginação e ordenação múltipla via `_page`, `_size`, `_order`
- [x] Publicação de eventos: `SaleCreated`, `SaleCancelled`, `SaleItemCancelled`, `SaleUpdated`
- [x] Testes unitários com xUnit, Bogus e NSubstitute
- [x] Estrutura modularizada (Domain, Application, ORM, WebApi)
- [x] Configuração de filas independentes com Rebus In-Memory

---

## 🚀 Como executar o projeto

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/) (opcional, para execução via container)

### 🔧 Build e execução local

```bash
# Rodar a aplicação
docker compose up
```

A API estará disponível em: `http://localhost:8081/swagger`

---

## 🧪 Executando os testes

```bash
# Executar os testes unitários
dotnet test Ambev.DeveloperEvaluation.sln
```

- Os testes seguem o padrão AAA
- Fakes com `Bogus`
- Mocks com `NSubstitute`

---

## 📚 Endpoints principais

### Vendas

- `POST /api/sales` - Criar venda
- `GET /api/sales?_page=1&_size=10&_order="total desc, createdAt"&branch=*Paulista&_minTotal=200` - Consultar com filtros, paginação e ordenação
- `PUT /api/sales/{id}` - Atualizar venda
- `PATCH /api/sales/{id}/cancel` - Cancelar venda
- `PATCH /api/sales/{saleId}/items/{productId}/cancel` - Cancelar item

### Autenticação
- `POST /auth/login`

---

## 📦 Estrutura de Diretórios

```
root
├── src/
│   ├── Ambev.DeveloperEvaluation.Domain
│   ├── Ambev.DeveloperEvaluation.Application
│   ├── Ambev.DeveloperEvaluation.ORM
│   └── Ambev.DeveloperEvaluation.WebApi
├── tests/
│   └── Ambev.DeveloperEvaluation.Tests
├── docker-compose.yml
└── README.md
```

---

## 📨 Eventos com Rebus

- `SaleCreatedEvent` → `sale-created-queue`
- `SaleCancelledEvent` → `sale-cancelled-queue`
- `SaleItemCancelledEvent` → `sale-item-cancelled-queue`
- `SaleUpdatedEvent` → `sale-updated-queue`

---

## 🧰 Tech Stack

- .NET 8 + C# 12
- PostgreSQL (EF Core)
- MediatR (CQRS)
- FluentValidation
- AutoMapper
- Rebus (In-Memory transport)
- xUnit + NSubstitute + Bogus
- Docker + Docker Compose
- Swagger para documentação

---

## 📄 Licença

Este projeto é apenas para fins de avaliação técnica e não possui licença comercial.
