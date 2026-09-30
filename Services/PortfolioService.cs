using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using portfolio.Data;

namespace portfolio.Services;

public class PortfolioService
{
    private static readonly object DatabaseInitializationLock = new();

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

                if (string.IsNullOrWhiteSpace(project.CaseStudySummary))
                {
                    project.CaseStudySummary = project.Description;
                    needsMigration = true;
                }

                if (string.IsNullOrWhiteSpace(project.Problem))
                {
                    project.Problem = project.Description;
                    needsMigration = true;
                }

                if (string.IsNullOrWhiteSpace(project.Solution))
                {
                    project.Solution = project.Description;
                    needsMigration = true;
                }

                if (string.IsNullOrWhiteSpace(project.Result))
                {
                    project.Result = project.Description;
                    needsMigration = true;
                }

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
        lock (DatabaseInitializationLock)
        {
            _dbContext.Database.EnsureCreated();
            MigrateLegacyJsonIfNeeded();
            EnsureSeedData();
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
