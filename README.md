# TechChallenge — FIAP Cloud Games (FCG)

API REST em .NET 8 para cadastro de usuários e biblioteca de jogos adquiridos.

Este é o projeto da **Fase 1** do Tech Challenge da FIAP Pós-Tech. A FIAP Cloud Games
será uma plataforma de venda de jogos digitais e gestão de servidores para partidas
online; esta fase entrega o MVP que serve de base para as próximas — matchmaking e
gerenciamento de servidores.

## Objetivos da fase

- Cadastrar usuários com validação de e-mail e senha segura
- Autenticar por token JWT com dois níveis de acesso (Usuário e Administrador)
- Permitir que administradores cadastrem jogos e criem promoções
- Manter a biblioteca de jogos adquiridos por cada usuário
- Garantir persistência com Entity Framework Core e migrations
- Aplicar DDD na organização do domínio, com Event Storming documentando os fluxos
- Cobrir as regras de negócio com testes automatizados

---

## Sumário

- [Pré-requisitos](#pré-requisitos)
- [Estrutura do projeto](#estrutura-do-projeto)
- [Subindo com Docker](#subindo-com-docker)
- [Rodando localmente](#rodando-localmente)
- [Primeiro acesso](#primeiro-acesso)
- [Endpoints](#endpoints)
- [Tratamento de erros](#tratamento-de-erros)
- [Testes](#testes)
- [Documentação DDD](#documentação-ddd)
- [Acessando o banco](#acessando-o-banco)
- [Connection strings](#connection-strings)
- [Problemas comuns](#problemas-comuns)

---

## Pré-requisitos

- Docker Desktop (inclui docker-compose)
- (Opcional) .NET 8 SDK para rodar localmente: https://dotnet.microsoft.com
- (Opcional) SQL Server tools (sqlcmd) ou cliente GUI (SSMS / Azure Data Studio)

## Estrutura do projeto

- `src/TechChallenge`: projeto web (ASP.NET Core) — controllers, middlewares e configuração
- `src/TechChallenge.Domain`: domínio — entidades, regras de negócio, services e exceções
- `src/TechChallenge.Infrastructure`: infra/EF Core — DbContext, repositórios e migrations
- `teste/TechChallengeUnitTests`: testes de unidade e cenários BDD
- `teste/TechChallengeIntegrationTests`: testes de integração entre as camadas
- `docs/`: documentação DDD (Event Storming)
- `docker-compose.yaml`: define serviços `sqlserver` e `webapp`
- `Dockerfile`: build e publish do projeto
- `appsettings.json`: connection string usada pela aplicação

## Subindo com Docker

1. Na raiz do repositório (onde está o `docker-compose.yaml`), execute:

   ```bash
   docker-compose up -d --build
   ```

2. Verificar containers:

   ```bash
   docker ps
   ```

3. Ver logs:

   ```bash
   docker-compose logs -f webapp
   ```

   ```bash
   docker-compose logs -f sqlserver
   ```

4. Acessar a API: http://localhost:5022/ — a raiz abre direto o Swagger.

5. Parar e remover containers/volumes:

   ```bash
   docker-compose down -v
   ```

## Rodando localmente

1. Ajuste a connection string em `appsettings.json` para apontar para o SQL desejado.
2. Restaurar e rodar:

   ```bash
   dotnet restore
   ```

   ```bash
   dotnet build
   ```

   ```bash
   dotnet run --project src/TechChallenge
   ```

A aplicação sobe em http://localhost:5022 e aplica as migrations automaticamente na
inicialização.

Se precisar subir só o banco e rodar a API pelo Visual Studio:

```bash
docker-compose up -d sqlserver
```

## Primeiro acesso

Na primeira inicialização, a aplicação cria um administrador para que seja possível
usar os endpoints protegidos:

| Login | Senha | Perfil |
|---|---|---|
| `admin` | `Admin@123` | Administrador |

Para obter o token:

```bash
curl -X POST http://localhost:5022/api/auth/login -H "Content-Type: application/json" -d "{\"login\":\"admin\",\"password\":\"Admin@123\"}"
```

A resposta traz o token e a data de expiração. No Swagger, clique em **Authorize** e
informe `Bearer {token}`.

> Ao trocar de usuário, volte no botão **Authorize** e cole o token novo — chamar
> `/api/auth/login` de novo não substitui o token que o Swagger está usando.

## Endpoints

Perfis: **Público** (sem token), **Cliente** e **Admin**.

### Autenticação — `/api/Auth`

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| POST | `/api/auth/register` | Público | Cadastra usuário (perfil Cliente) |
| POST | `/api/auth/login` | Público | Autentica e devolve o token JWT |

### Jogos — `/Game`

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| GET | `/Game` | Cliente, Admin | Lista os jogos |
| GET | `/Game/{id}` | Cliente, Admin | Consulta um jogo |
| POST | `/Game` | Admin | Cadastra um jogo |
| PUT | `/Game/{id}` | Admin | Atualiza um jogo |
| DELETE | `/Game/{id}` | Admin | Remove um jogo |

### Biblioteca do usuário — `/api/LibraryOfGames`

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| POST | `/api/LibraryOfGames/{gameId}` | Autenticado | Adquire um jogo |
| GET | `/api/LibraryOfGames` | Autenticado | Lista a biblioteca do usuário logado |

### Promoções — `/api/Promotions`

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| GET | `/api/Promotions` | Público | Lista as promoções |
| GET | `/api/Promotions/{id}` | Público | Consulta uma promoção |
| POST | `/api/Promotions` | Admin | Cria uma promoção |
| DELETE | `/api/Promotions/{id}` | Admin | Remove uma promoção |

### Usuários — `/api/Users`

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| GET | `/api/Users` | Admin | Lista os usuários |
| GET | `/api/Users/{id}` | Admin | Consulta um usuário |
| DELETE | `/api/Users/{id}` | Admin | Remove um usuário |

### Regras de cadastro

- **E-mail** precisa ter formato válido e ser único
- **Login** precisa ser único
- **Senha** precisa ter no mínimo 8 caracteres, com ao menos uma letra, um número e
  um caractere especial — e é gravada com hash BCrypt, nunca em texto puro

## Tratamento de erros

As falhas da aplicação passam por um middleware central e voltam no formato
**ProblemDetails (RFC 7807)**, com `content-type: application/problem+json`:

```json
{
  "title": "Recurso não encontrado",
  "status": 404,
  "detail": "Jogo 999 não encontrado.",
  "instance": "/Game/999",
  "traceId": "0HNO26PBSVG6H:00000001"
}
```

| Status | Quando acontece | Corpo |
|---|---|---|
| 400 | Dados inválidos (senha fraca, e-mail malformado, preço negativo) | ProblemDetails |
| 401 | Credenciais erradas no login | ProblemDetails |
| 404 | Recurso não encontrado | ProblemDetails |
| 409 | Conflito com o estado atual (login/e-mail já cadastrado, jogo já na biblioteca) | ProblemDetails |
| 500 | Erro inesperado — mensagem genérica; o stack trace fica só no log | ProblemDetails |
| 401 | Token ausente, inválido ou expirado | vazio + `WWW-Authenticate` |
| 403 | Perfil sem permissão para o endpoint | vazio |

O `traceId` da resposta é o mesmo gravado no log estruturado (Serilog), o que permite
localizar no log exatamente o erro que o usuário viu. Em erros 500 a mensagem original
nunca é exposta ao cliente.

### Sobre os 401 e 403 sem corpo

As duas últimas linhas da tabela não passam pelo middleware: `UseAuthentication` e
`UseAuthorization` interrompem o pipeline antes dos controllers, sem lançar exceção.
São respostas geradas pelo próprio ASP.NET Core.

Isso está de acordo com a RFC 9110, que exige apenas o header `WWW-Authenticate` no 401
— e ele vem preenchido, distinguindo os dois casos:

```
Sem token:       WWW-Authenticate: Bearer
Token inválido:  WWW-Authenticate: Bearer error="invalid_token"
```

Corpo em respostas de erro é opcional pela especificação. Mantivemos o comportamento
padrão do framework: quem consome a API deve tratar 401 e 403 pelo status e pelo header,
e os demais erros pelo corpo em ProblemDetails.

## Testes

Rodar tudo:

```bash
dotnet test
```

Somente os testes de unidade (mais rápidos, sem banco):

```bash
dotnet test teste/TechChallengeUnitTests
```

Somente os testes de integração:

```bash
dotnet test teste/TechChallengeIntegrationTests
```

Com relatório de cobertura:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### O que cada suíte cobre

| Suíte | O que testa |
|---|---|
| **Unidade** | Regras de negócio das entidades e services, com repositórios mockados (Moq), e o middleware de exceções isoladamente |
| **BDD** | Cenários de cadastro de usuário escritos em Gherkin e executados com Reqnroll — ver `teste/TechChallengeUnitTests/Features/UserRegistration.feature` |
| **Integração** | Services + repositórios + Entity Framework reais sobre banco em memória, verificando que o dado chega ao banco e que as camadas conversam |

### Testes manuais da API

Com a aplicação no ar, importe no Postman a coleção
`postman/TechChallenge-Demo.postman_collection.json`.

São **18 requisições** na ordem de uma apresentação, cobrindo cada requisito do desafio
uma vez: cadastro com validação, autenticação JWT, os dois níveis de acesso, catálogo e
promoções pelo administrador, e a biblioteca do usuário.

Os tokens de administrador e de cliente e o id do jogo são capturados automaticamente —
basta importar e rodar na ordem (*Run collection*). Cada requisição valida o status e,
nos erros da aplicação, o formato ProblemDetails. A coleção pode ser executada várias
vezes seguidas sem recriar o banco.

Para rodar pela linha de comando:

```bash
npx newman run postman/TechChallenge-Demo.postman_collection.json
```

## Documentação DDD

O Event Storming dos fluxos está em `docs/`:

- `docs/event-storming.md` — documento completo com os fluxos de criação de usuários,
  criação de jogos e aquisição, além de agregados, contextos delimitados e linguagem
  ubíqua
- `docs/event-storming-board.html` — versão visual do board

## Acessando o banco

- Conexão via host (GUI: SSMS/Azure Data Studio):
  - Server name: localhost,1433
  - Autenticação: SQL Server Authentication
  - User: sa
  - Password: YourStrong!Passw0rd

- Usando sqlcmd dentro do container:

  ```bash
  docker exec -it TechChallengeSQL /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd' -Q "SELECT name FROM sys.databases;"
  ```

## Connection strings

- Quando a aplicação roda em container (mesmo docker-compose):

  ```
  Server=sqlserver,1433;Database=TechChallengeDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;
  ```

- Quando a aplicação roda no host e o SQL está no container (porta 1433 exposta):

  ```
  Server=localhost,1433;Database=TechChallengeDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;
  ```

> As credenciais acima são de desenvolvimento e estão versionadas de propósito, para
> o projeto subir com um comando. Em produção elas viriam de variáveis de ambiente ou
> de um cofre de segredos.

## Problemas comuns

- Se o SQL Server demora a inicializar, verifique `docker-compose logs sqlserver` e aguarde "ready for client connections".
- Erros de certificado TLS: use `TrustServerCertificate=True` na connection string.
- Se o build falhar por não encontrar projetos, confirme que está rodando o `docker-compose` na raiz do repositório.
- Se os endpoints protegidos responderem 401 mesmo com token, confirme que o valor foi
  informado como `Bearer {token}` e que o token não expirou (validade de 60 minutos).

---

## Stack

.NET 8 · ASP.NET Core · Entity Framework Core · SQL Server · JWT · BCrypt · Serilog ·
Swagger · xUnit · Moq · Reqnroll
