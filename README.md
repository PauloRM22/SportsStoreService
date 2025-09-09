# Arlequim Stack — Desafio Técnico Backend
API para Gestão de Estoque e Produtos

## 📌 Visão Geral
Projeto desenvolvido como parte de um **teste técnico da Arlequim Stack**.  
A API implementa **cadastro de usuários**, **login (JWT)**, **CRUD de produtos**, **controle de estoque** e **emissão/confirm** de **pedidos**, utilizando **.NET 8**, **EF Core/SQL Server**, **RabbitMQ** (mensageria – bônus) e **OpenTelemetry → Datadog** (tracing/APM – bônus).

Arquitetura em camadas (`Domain`, `Application`, `Infrastructure`, `API`) seguindo **DDD**, **SOLID**, tratamento consistente de erros e **testes automatizados** (unitários e integração).

---

## 🛠️ Tecnologias & Bibliotecas
- **.NET 8** + ASP.NET Core
- **Entity Framework Core 9** (migrations aplicadas em runtime)
- **SQL Server** (Docker)
- **RabbitMQ + MassTransit** (publicação/consumo de eventos)
- **JWT Authentication** (políticas: **AdminOnly** / **VendedorOnly**)
- **OpenTelemetry** (ASP.NET + HttpClient + SqlClient) → **Datadog Agent** via **OTLP**
- **xUnit + FluentAssertions + Moq** (testes)

---

## 🚀 Execução via Docker

### Pré-requisitos
- Docker Desktop instalado e em execução

### Subir os serviços
Na pasta raiz do repo:
```bash
docker compose up -d --build
```

### Serviços disponíveis
- **API (Swagger)**: http://localhost:8080/swagger  
- **RabbitMQ Management**: http://localhost:15672 (user: `app`, pass: `app`)  
- **SQL Server**: `localhost,1433` (sa / `123Strong!`)  
- **Datadog Agent (OTLP)**: exposto nas portas `4317` (gRPC) e `4318` (HTTP)

> A API sobe em **Development** para permitir o endpoint de **bootstrap** do primeiro usuário.

---

## 🔑 Autenticação (JWT)
- **Administrador**: gerencia **Usuários**, **Produtos**, **Estoque**  
- **Vendedor**: cria **Pedidos** e executa **Confirmação** (baixa de estoque)  
- Login via `POST /api/Auth/login` → retorna **Bearer Token** para usar no Swagger

---

## 🧭 Roteiro E2E para QA (copiar e colar os JSONs)

### 0) Bootstrap do Admin (somente se ainda não existir usuário)
`POST /api/usuarios/bootstrap` *(AllowAnonymous em Development; bloqueia após existir usuário)*  
```json
{
  "nome": "Administrador",
  "email": "admin@sportstore.com",
  "senha": "123456",
  "tipoUsuario": "Administrador"
}
```

> Depois do primeiro usuário, novas criações são via `POST /api/usuarios` **com Admin autenticado**.

---

### 1) Login (Admin)
`POST /api/Auth/login`
```json
{
  "email": "admin@sportstore.com",
  "senha": "123456"
}
```
Use o token no botão **Authorize** do Swagger (Bearer).

---

### 2) Criar Produto (Admin)
`POST /api/produtos`
```json
{
  "nome": "Bola Oficial",
  "descricao": "Bola de futebol campo",
  "preco": 199.9
}
```

---

### 3) Entrada de Estoque (Admin)
`POST /api/estoque/entrada`
```json
{
  "produtoId": "<ID do produto>",
  "quantidade": 50,
  "numeroNotaFiscal": "NF-12345",
  "observacao": "Lote inicial"
}
```

---

### 4) Criar Vendedor (Admin)
`POST /api/usuarios`
```json
{
  "nome": "João Vendedor",
  "email": "vendedor@sportstore.com",
  "senha": "123456",
  "tipoUsuario": "Vendedor"
}
```

---

### 5) Login (Vendedor)
`POST /api/Auth/login`
```json
{
  "email": "vendedor@sportstore.com",
  "senha": "123456"
}
```
Use o token no botão **Authorize** do Swagger (Bearer).
---

### 6) Criar Pedido (Vendedor)
`POST /api/pedidos`
```json
{
  "clienteDocumento": "123.456.789-00",
  "vendedorNome": "Carlos",
  "itens": [
    {
      "produtoId": "<ID do produto>",
      "quantidade": 2
    }
  ]
}
```

---

### 7) Confirmar Pedido (Vendedor) — baixa de estoque
`POST /api/pedidos/{id}/confirmar`  
**Resultado esperado**:
- **200** → pedido confirmado e **estoque baixado**
- **422** → **estoque insuficiente** (mensagem de validação clara)

> ⚠️ **Nota sobre H5 (Pedidos):**  
> O enunciado original sugere que o pedido não deve ser criado se não houver estoque.  
> Nesta implementação, o pedido é salvo primeiro como **rascunho**, e a checagem de estoque ocorre na **confirmação**. Assim garantimos que não haja baixa incorreta e ainda permitimos ao vendedor registrar pedidos preliminares.  
> Decidimos que **a efetivação (baixa de estoque)** só acontece na confirmação, garantindo consistência e robustez em cenários concorrentes.

---

## 📨 Mensageria (Bônus — RabbitMQ)

### O que acontece
- Ao confirmar pedido, a API publica **`PedidoConfirmadoEvent`** via MassTransit.  
- **Consumer**: `PedidoConfirmadoConsumer` escuta a fila `notifier-pedido-confirmado` e registra log:
  ```
  Pedido confirmado #<codigo> (R$<valor>) para <clienteDocumento> por <vendedorNome>. Itens: <qtd>
  ```

### Como verificar
1. Painel: http://localhost:15672 → **Queues** → `notifier-pedido-confirmado`  
2. Logs da API:
   ```bash
   docker logs -f sportstore_api
   ```

> Credenciais do RabbitMQ: `app` / `app` (configuradas no `docker-compose.yml`).

---

## 📊 Tracing/APM (Bônus — OpenTelemetry → Datadog)

### Como está implementado
- Extension `AddObservability` (API) configura **OpenTelemetry Tracing** com:
  - `aspnet_core.request` (ASP.NET Core)
  - `sql.client` (SqlClient/EF)
  - `http.client` (se usado)
- Export via **OTLP (gRPC)** para o **Datadog Agent** (`http://datadog:4317`) do `docker-compose`.

### Ativação
1. No `docker-compose.yml`, defina sua API Key:
   ```yaml
   datadog:
     environment:
       - DD_API_KEY=${DD_API_KEY}
       - DD_SITE=datadoghq.com
       - DD_APM_ENABLED=true
       - DD_OTLP_CONFIG_RECEIVER_PROTOCOLS_GRPC=1
       - DD_OTLP_CONFIG_RECEIVER_PROTOCOLS_HTTP=1
   ```
2. Suba/reconstrua:
   ```bash
   docker compose up -d --build
   ```
3. Gere tráfego (via Swagger).  
4. No Datadog: **APM → Services** → `sportsstore-api` (ver traces e spans).

> Dica: status do Agent → `docker exec -it sportstore_datadog agent status`

---

## ✅ Histórias de Usuário atendidas
**H1 – Cadastro de Usuários**  
- Criar, editar, listar, consultar, excluir; perfis **Administrador** e **Vendedor**; e-mail **único**; senha min. **6**.  
- **Bootstrap** do primeiro Admin em Development: `POST /api/usuarios/bootstrap`.

**H2 – Login**  
- Via e-mail/senha; retorna **token JWT**.

**H3 – Gerenciamento de Produtos**  
- CRUD completo; produto com **nome**, **descrição**, **preço**.

**H4 – Controle de Estoque**  
- **Entrada** com **quantidade** e **nº de nota fiscal** (auditoria).  
- Consulta de **saldo** por produto (Swagger).

**H5 – Emissão de Pedidos**  
- Criar **rascunho** com itens e documento do cliente.  
- **Confirmar**: baixa de estoque.  
- Se faltar estoque → **não confirma** (erro 422 com mensagem).  
- **Decisão técnica**: não bloquear na criação (rascunho), somente na confirmação.

**Diferenciais (Bônus)**  
- **RabbitMQ/MassTransit** (publish/consume de `PedidoConfirmadoEvent`).  
- **Tracing/APM** com **OpenTelemetry → Datadog** (OTLP).

---

## 🧪 Testes
Rodar todos os testes:
```bash
dotnet test
```

---

## 🧱 Decisões Técnicas
- **Baixa de estoque** ocorre **na confirmação** do pedido (não no rascunho).  
- **Estoque insuficiente** → confirmação falha (422) com mensagem amigável.  
- **Bootstrap** do primeiro Admin via endpoint dedicado, somente em **Development**.

---

## 🩺 Troubleshooting rápido
- **PowerShell**: use `;` para encadear comandos (`dotnet restore; dotnet build`).  
- **YAML**: sem **tabs**; só **espaços**. Erro comum: `yaml: invalid trailing UTF-8 octet`.  
- **RabbitMQ**: se `guest/guest` não conectar entre containers, use usuário próprio (já configurado `app/app`).  
- **MassTransit**: registre **uma vez** (mantido em `ServiceExtensions`).  
- **Datadog**: sem tráfego não há traces; confira a chave `DD_API_KEY` e o log do Agent:
  ```bash
  docker logs -f sportstore_datadog
  ```

---

## 📂 Estrutura (resumo)
```
SportsStoreService/           # API (Controllers, Middlewares, Extensions, Observability)
StoreService.Application/     # Use cases, DTOs, Validators, Ports, Mensageria (contratos)
StoreService.Domain/          # Entidades, Enums
StoreService.Infrastructure/  # EF Core, Repositórios, DI, EventPublisher, Migrations
StoreService.Tests/           # Unit tests
StoreService.IntegrationTests/# Integration/E2E tests
docker-compose.yml
README.md
```
