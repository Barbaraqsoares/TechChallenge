# Etapa 1: build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia todo o código da solução para o contexto de build
COPY . .

# Restaura dependências a partir da solução (projetos estão em src/)
RUN dotnet restore TechChallenge.sln

# Compila e publica o projeto web (localizado em src/TechChallenge)
RUN dotnet publish src/TechChallenge/TechChallenge.csproj -c Release -o /app

# Etapa 2: imagem final para rodar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Expõe a porta padrão do ASP.NET
EXPOSE 80

# Comando de inicialização
ENTRYPOINT ["dotnet", "TechChallenge.dll"]