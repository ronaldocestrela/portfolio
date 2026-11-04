# Estágio de compilação
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["portfolio.csproj", "./"]
RUN dotnet restore "portfolio.csproj"

COPY . .
RUN dotnet publish "portfolio.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio de desenvolvimento (utilizado com docker-compose.dev.yml)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dev
WORKDIR /src
COPY ["portfolio.csproj", "./"]
RUN dotnet restore "portfolio.csproj"
COPY . .
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://0.0.0.0:3001
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
EXPOSE 3001
CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:3001", "--no-hot-reload"]

# Estágio final de produção (padrão)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Cria os diretórios necessários para persistência de dados e uploads
RUN mkdir -p /app/data /app/wwwroot/uploads

EXPOSE 3001

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:3001

ENTRYPOINT ["dotnet", "portfolio.dll"]
