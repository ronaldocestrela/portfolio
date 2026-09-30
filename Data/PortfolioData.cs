using System.Globalization;

namespace portfolio.Data;

public class ExperienceItem
{
    public string Role { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Tech { get; set; } = [];

    public IReadOnlyList<string> DistinctTech => GetUniqueValues(Tech);

    private static IReadOnlyList<string> GetUniqueValues(IEnumerable<string>? values)
    {
        if (values is null)
        {
            return [];
        }

        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static DateTime? ParseDate(string? value)
    {
        var normalized = NormalizeMonthYear(value);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return null;
        }

        if (DateTime.TryParse(normalized, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            return parsedDate;
        }

        return null;
    }

    public static string NormalizeMonthYear(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var trimmed = value.Trim();

        if (trimmed.Contains('/'))
        {
            return trimmed;
        }

        if (trimmed.Length == 4 && int.TryParse(trimmed, out var yearOnly))
        {
            return $"01/{yearOnly}";
        }

        if (trimmed.Length == 7 && trimmed[4] == '-' &&
            DateTime.TryParseExact(trimmed, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var monthYear))
        {
            return monthYear.ToString("MM/yyyy", CultureInfo.InvariantCulture);
        }

        if (DateTime.TryParse(trimmed, out var parsedDate))
        {
            return parsedDate.ToString("MM/yyyy", CultureInfo.InvariantCulture);
        }

        return trimmed;
    }

    public string DisplayPeriod
    {
        get
        {
            var start = NormalizeMonthYear(StartDate);
            var end = NormalizeMonthYear(EndDate);

            if (!string.IsNullOrWhiteSpace(start) || !string.IsNullOrWhiteSpace(end))
            {
                if (string.IsNullOrWhiteSpace(start) && string.IsNullOrWhiteSpace(end))
                {
                    return string.Empty;
                }

                if (string.IsNullOrWhiteSpace(end))
                {
                    return string.IsNullOrWhiteSpace(start) ? "Atual" : $"{start} - Atual";
                }

                return string.IsNullOrWhiteSpace(start) ? end : $"{start} - {end}";
            }

            if (string.IsNullOrWhiteSpace(Period))
            {
                return string.Empty;
            }

            return Period.Trim();
        }
    }
}

public class ProjectItem
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Stack { get; set; } = [];
    public string Link { get; set; } = string.Empty;
    public List<string> Images { get; set; } = [];
    public string DisplayImage { get; set; } = string.Empty;
    public string CaseStudySummary { get; set; } = string.Empty;
    public string Problem { get; set; } = string.Empty;
    public string Solution { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public List<string> KeyMetrics { get; set; } = [];

    public IReadOnlyList<string> DistinctStack => GetUniqueValues(Stack);
    public IReadOnlyList<string> ValidImages => GetUniqueValues(Images).Take(5).ToList();

    public string DisplayImageUrl
    {
        get
        {
            var validImages = ValidImages;

            if (validImages.Count == 0)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(DisplayImage) &&
                validImages.Any(item => string.Equals(item, DisplayImage, StringComparison.OrdinalIgnoreCase)))
            {
                return DisplayImage;
            }

            return validImages[0];
        }
    }

    private static IReadOnlyList<string> GetUniqueValues(IEnumerable<string>? values)
    {
        if (values is null)
        {
            return [];
        }

        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

public class PortfolioData
{
    public string Name { get; set; } = "Seu Nome";
    public string Title { get; set; } = "Desenvolvedor Full-Stack";
    public string Location { get; set; } = "Brasil";
    public string Email { get; set; } = "seuemail@email.com";
    public string Summary { get; set; } = "Crio experiências digitais com foco em performance, usabilidade e resultados para produtos que precisam entregar valor real.";
    public string Github { get; set; } = "https://github.com/seu-usuario";
    public string LinkedIn { get; set; } = "https://linkedin.com/in/seu-usuario";
    public string HeroBadge { get; set; } = "Disponível para freelas e projetos";

    public int YearsOfExperience
    {
        get
        {
            var oldestStartDate = Experience
                .Select(item => ExperienceItem.ParseDate(item.StartDate))
                .Where(date => date.HasValue)
                .Min();

            if (!oldestStartDate.HasValue)
            {
                return 0;
            }

            var today = DateTime.Today;
            var years = today.Year - oldestStartDate.Value.Year;

            if (today < oldestStartDate.Value.AddYears(years))
            {
                years--;
            }

            return Math.Max(years, 0);
        }
    }

    public List<ExperienceItem> Experience { get; set; } = new()
    {
        new ExperienceItem
        {
            Role = "Desenvolvedor Front-end",
            Company = "Empresa A",
            StartDate = "03/2023",
            Description = "Construção de interfaces modernas, responsivas e acessíveis para produtos digitais com foco em conversão e experiência do usuário.",
            Tech = ["Next.js", "TypeScript", "Tailwind"]
        },
        new ExperienceItem
        {
            Role = "Desenvolvedor Full-Stack",
            Company = "Empresa B",
            StartDate = "01/2021",
            EndDate = "12/2023",
            Description = "Implementação de dashboards, integrações de API e otimização de fluxos de trabalho internos.",
            Tech = ["Node.js", "React", "PostgreSQL"]
        }
    };

    public List<ProjectItem> Projects { get; set; } = new()
    {
        new ProjectItem
        {
            Name = "Dashboard de Vendas",
            Description = "Painel para acompanhamento de métricas, relatórios e visão operacional em tempo real.",
            Stack = ["Next.js", "Supabase", "Charts"],
            Link = "https://github.com/seu-usuario/dashboard-vendas",
            CaseStudySummary = "Reestruturamos a operação comercial com painel em tempo real, aceleração na análise e maior qualidade nas decisões.",
            Problem = "Os times de vendas e operação estavam usando relatórios fragmentados e dados inconsistentes para acompanhar desempenho.",
            Solution = "Criei uma arquitetura consolidada com dashboards interativos, métricas automatizadas e fluxos de revisão em tempo real.",
            Result = "A equipe reduziu o tempo de análise operacional e passou a responder com mais agilidade às oscilações do negócio.",
            KeyMetrics = ["42% menos tempo de análise", "+18k registros monitorados", "99.99% de disponibilidade"]
        },
        new ProjectItem
        {
            Name = "Plataforma de Leads",
            Description = "Sistema para captação, qualificação e organização de oportunidades de negócio.",
            Stack = ["React", "Node.js", "MongoDB"],
            Link = "https://github.com/seu-usuario/plataforma-leads",
            CaseStudySummary = "Aumentei a eficiência do time comercial com processo de lead qualification centralizado e automatizado.",
            Problem = "Os leads eram capturados em canais diferentes, com baixa rastreabilidade e pouca visibilidade do estágio atual.",
            Solution = "Desenhei uma plataforma com pipeline visual, regras de qualificação e histórico de interações por oportunidade.",
            Result = "A operação passou a priorizar melhor os prospects e reduzir retrabalho, com acompanhamento mais claro em cada etapa.",
            KeyMetrics = ["3x mais velocidade no rastreio", "+65% melhoria no follow-up", "+90% visibilidade do pipeline"]
        }
    };

    public static PortfolioData CreateDefault() => new();
}
