# TechChallenge


- Docker Desktop (inclui docker-compose)
- (Opcional) .NET 8 SDK para rodar localmente: https://dotnet.microsoft.com
- (Opcional) SQL Server tools (sqlcmd) ou cliente GUI (SSMS / Azure Data Studio)

- src/TechChallenge: projeto web (ASP.NET Core)
- src/TechChallenge.Domain: domínio
- src/TechChallenge.Infrastructure: infra/EF Core
- docker-compose.yaml: define serviços `sqlserver` e `webapp`
- Dockerfile: build e publish do projeto
- appsettings.json: connection string usada pela aplicação

1. Na raiz do repositório (onde está o `docker-compose.yaml`), execute:

   docker-compose up -d --build

2. Verificar containers:

   docker ps

3. Ver logs:

   docker-compose logs -f webapp
   docker-compose logs -f sqlserver

4. Acessar a API (exemplo): http://localhost:8080/ (porta mapeada no compose)

5. Parar e remover containers/volumes:

   docker-compose down -v

- Conexão via host (GUI: SSMS/Azure Data Studio):
  - Server name: localhost,1433
  - Autenticação: SQL Server Authentication
  - User: sa
  - Password: YourStrong!Passw0rd

- Usando sqlcmd dentro do container:

  docker exec -it TechChallengeSQL /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd' -Q "SELECT name FROM sys.databases;"

- Quando a aplicação roda em container (mesmo docker-compose):
  Server=sqlserver,1433;Database=TechChallengeDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;

- Quando a aplicação roda no host e o SQL está no container (porta 1433 exposta):
  Server=localhost,1433;Database=TechChallengeDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;

1. Ajuste a connection string em `appsettings.json` para apontar para o SQL desejado.
2. Restaurar e rodar:

   dotnet restore
   dotnet build
   dotnet run --project src/TechChallenge

- Se o SQL Server demora a inicializar, verifique `docker-compose logs sqlserver` e aguarde "ready for client connections".
- Erros de certificado TLS: use `TrustServerCertificate=True` na connection string.
- Se o build falhar por não encontrar projetos, confirme que está rodando o `docker-compose` na raiz do repositório.

