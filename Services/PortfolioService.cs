using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using portfolio.Data;

namespace portfolio.Services;

public class PortfolioService
{
    private static readonly object DatabaseInitializationLock = new();
    private static bool _isInitialized;

    private readonly PortfolioDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly string _legacyFilePath;

    public PortfolioService(PortfolioDbContext dbContext, IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _environment = environment;
        _legacyFilePath = Path.Combine(_environment.ContentRootPath, "data", "portfolio.json");

        InitializeDatabase();
    }

    public PortfolioData GetPortfolio()
    {
        var record = _dbContext.PortfolioRecords.OrderBy(x => x.Id).FirstOrDefault();

        if (record is null || string.IsNullOrWhiteSpace(record.Content))
        {
            var defaultData = PortfolioData.CreateDefault();
            SavePortfolio(defaultData);
            return defaultData;
        }

        try
        {
            var portfolio = JsonSerializer.Deserialize<PortfolioData>(record.Content) ?? PortfolioData.CreateDefault();

            foreach (var experience in portfolio.Experience)
            {
                experience.Tech = experience.DistinctTech.ToList();

                if (string.IsNullOrWhiteSpace(experience.StartDate) && string.IsNullOrWhiteSpace(experience.EndDate) && !string.IsNullOrWhiteSpace(experience.Period))
                {
                    var parts = experience.Period.Split('-', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length > 0)
                    {
                        experience.StartDate = ExperienceItem.NormalizeMonthYear(parts[0]);
                    }

                    if (parts.Length > 1)
                    {
                        var endValue = parts[1].Trim();
                        if (endValue.Equals("Atual", StringComparison.OrdinalIgnoreCase))
                        {
                            experience.EndDate = string.Empty;
                        }
                        else
                        {
                            experience.EndDate = ExperienceItem.NormalizeMonthYear(endValue);
                        }
                    }

                    experience.Period = string.Empty;
                }
            }

            var needsMigration = false;

            foreach (var project in portfolio.Projects)
            {
                project.Stack = project.DistinctStack.ToList();
                project.Images = project.ValidImages.ToList();
                project.NormalizeDetailSections();

                if (project.KeyMetrics is null || project.KeyMetrics.Count == 0)
                {
                    project.KeyMetrics = ["Resultado entregue", "Tempo otimizado", "Melhor visibilidade operacional"];
                    needsMigration = true;
                }

                if (project.Images.Count == 0)
                {
                    project.DisplayImage = string.Empty;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(project.DisplayImage) ||
                    !project.Images.Any(image => string.Equals(image, project.DisplayImage, StringComparison.OrdinalIgnoreCase)))
                {
                    project.DisplayImage = project.DisplayImageUrl;
                    needsMigration = true;
                }
            }

            if (needsMigration)
            {
                SavePortfolio(portfolio);
            }

            return portfolio;
        }
        catch
        {
            var defaultData = PortfolioData.CreateDefault();
            SavePortfolio(defaultData);
            return defaultData;
        }
    }

    public void SavePortfolio(PortfolioData data)
    {
        var record = _dbContext.PortfolioRecords.OrderBy(x => x.Id).FirstOrDefault() ?? new PortfolioRecord();
        record.Content = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

        if (record.Id == 0)
        {
            _dbContext.PortfolioRecords.Add(record);
        }

        _dbContext.SaveChanges();
    }

    private void InitializeDatabase()
    {
        if (_isInitialized)
        {
            return;
        }

        lock (DatabaseInitializationLock)
        {
            if (_isInitialized)
            {
                return;
            }

            EnsureDatabaseDirectoryExists();
            _dbContext.Database.EnsureCreated();
            MigrateLegacyJsonIfNeeded();
            EnsureSeedData();
            _isInitialized = true;
        }
    }

    private void EnsureDatabaseDirectoryExists()
    {
        try
        {
            var connection = _dbContext.Database.GetDbConnection();
            var builder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connection.ConnectionString);
            if (!string.IsNullOrWhiteSpace(builder.DataSource) &&
                !builder.DataSource.StartsWith(":memory:", StringComparison.OrdinalIgnoreCase) &&
                !builder.DataSource.StartsWith("file::memory:", StringComparison.OrdinalIgnoreCase))
            {
                var fullDbPath = Path.IsPathRooted(builder.DataSource)
                    ? builder.DataSource
                    : Path.GetFullPath(Path.Combine(_environment.ContentRootPath, builder.DataSource));

                var dbDirectory = Path.GetDirectoryName(fullDbPath);
                if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
                {
                    Directory.CreateDirectory(dbDirectory);
                }
            }
        }
        catch
        {
            var defaultDataDir = Path.Combine(_environment.ContentRootPath, "data");
            if (!Directory.Exists(defaultDataDir))
            {
                Directory.CreateDirectory(defaultDataDir);
            }
        }
    }

    private void EnsureSeedData()
    {
        if (_dbContext.PortfolioRecords.Any())
        {
            return;
        }

        SavePortfolio(PortfolioData.CreateDefault());
    }

    private void MigrateLegacyJsonIfNeeded()
    {
        if (_dbContext.PortfolioRecords.Any() || !File.Exists(_legacyFilePath))
        {
            return;
        }

        var json = File.ReadAllText(_legacyFilePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        var portfolio = JsonSerializer.Deserialize<PortfolioData>(json);
        if (portfolio is null)
        {
            return;
        }

        SavePortfolio(portfolio);
    }
}
