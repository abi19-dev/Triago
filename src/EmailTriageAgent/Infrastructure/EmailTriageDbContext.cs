using EmailTriageAgent.Domain;
using Microsoft.EntityFrameworkCore;

namespace EmailTriageAgent.Infrastructure;

public sealed class EmailTriageDbContext : DbContext
{
    public EmailTriageDbContext(DbContextOptions<EmailTriageDbContext> options)
        : base(options)
    {
    }

    public DbSet<EmailMessage> EmailMessages => Set<EmailMessage>();
    public DbSet<Prediction> Predictions => Set<Prediction>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ModelVersion> ModelVersions => Set<ModelVersion>();
    public DbSet<SystemSettings> SystemSettings => Set<SystemSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailMessage>()
            .HasIndex(m => m.Status);

        modelBuilder.Entity<EmailMessage>()
            .HasOne(m => m.Prediction)
            .WithOne(p => p.EmailMessage)
            .HasForeignKey<Prediction>(p => p.EmailMessageId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.EmailMessage)
            .WithMany(m => m.Reviews)
            .HasForeignKey(r => r.EmailMessageId);

    }
}
