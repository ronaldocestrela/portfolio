FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dev

WORKDIR /src

COPY . .
RUN dotnet restore "portfolio.csproj"

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://0.0.0.0:3001
ENV DOTNET_USE_POLLING_FILE_WATCHER=1

EXPOSE 3001

CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:3001"]
