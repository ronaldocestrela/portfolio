using Microsoft.EntityFrameworkCore;

namespace portfolio.Data;

public class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
        : base(options)
    {
    }

    public DbSet<PortfolioRecord> PortfolioRecords => Set<PortfolioRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PortfolioRecord>()
            .Property(x => x.Content)
            .HasColumnType("TEXT");
    }
}

public class PortfolioRecord
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
}
