# Portfólio em Blazor

Aplicação de portfólio e painel administrativo desenvolvida em .NET 10 + Blazor.

## Como rodar

### Localmente

```bash
dotnet restore
dotnet run
```

### Docker em Produção

```bash
docker compose up -d --build
```

### Docker em Desenvolvimento (com Hot Reload)

```bash
docker compose -f docker-compose.dev.yml up -d --build
```

A aplicação fica disponível em:
- http://localhost:5000
- http://localhost:3001 (configuração atual do projeto)

## Estrutura principal

- `Components/Pages/Home.razor` — landing page do portfólio
- `Components/Pages/Admin.razor` — painel para editar conteúdo
- `Services/PortfolioService.cs` — leitura e gravação do conteúdo em SQLite
- `Data/PortfolioDbContext.cs` — contexto do banco de dados SQLite
- `Data/PortfolioData.cs` — modelos de dados
- `data/portfolio.db` — banco local do site

## Como editar o conteúdo

Acesse a rota `/admin` para alterar:
- nome, cargo e resumo
- experiências profissionais
- projetos
- links e informações de contato

Os dados são persistidos em SQLite no arquivo `data/portfolio.db`.
