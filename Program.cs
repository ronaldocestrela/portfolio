using Microsoft.EntityFrameworkCore;
using portfolio.Components;
using portfolio.Data;
using portfolio.Services;

LoadDotEnv();

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

var rawConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=data/portfolio.db";

var connectionStringBuilder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(rawConnectionString);
if (!string.IsNullOrWhiteSpace(connectionStringBuilder.DataSource) &&
    !connectionStringBuilder.DataSource.StartsWith(":memory:", StringComparison.OrdinalIgnoreCase) &&
    !connectionStringBuilder.DataSource.StartsWith("file::memory:", StringComparison.OrdinalIgnoreCase))
{
    var fullDbPath = Path.IsPathRooted(connectionStringBuilder.DataSource)
        ? connectionStringBuilder.DataSource
        : Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, connectionStringBuilder.DataSource));

    var dbDirectory = Path.GetDirectoryName(fullDbPath);
    if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
    {
        Directory.CreateDirectory(dbDirectory);
    }

    connectionStringBuilder.DataSource = fullDbPath;
}

var connectionString = connectionStringBuilder.ToString();

builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 32 * 1024 * 1024; // 32MB
    });

builder.Services.AddScoped<PortfolioService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

var disableHttpsRedirection = string.Equals(
    builder.Configuration["DISABLE_HTTPS_REDIRECTION"] ?? Environment.GetEnvironmentVariable("DISABLE_HTTPS_REDIRECTION"),
    "true",
    StringComparison.OrdinalIgnoreCase);

if (!app.Environment.IsDevelopment() && !disableHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static void LoadDotEnv()
{
    var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (!File.Exists(envFile))
    {
        return;
    }

    foreach (var rawLine in File.ReadAllLines(envFile))
    {
        var line = rawLine.Trim();
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
        {
            continue;
        }

        var separatorIndex = line.IndexOf('=');
        if (separatorIndex <= 0)
        {
            continue;
        }

        var key = line[..separatorIndex].Trim();
        var value = line[(separatorIndex + 1)..].Trim();

        if ((value.StartsWith('"') && value.EndsWith('"')) ||
            (value.StartsWith('\'') && value.EndsWith('\'')))
        {
            value = value[1..^1];
        }

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
