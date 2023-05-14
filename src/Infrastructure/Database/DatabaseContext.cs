using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Session> Sessions { get; set; }

    public DbSet<Interaction> Interactions { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Created).IsRequired();
            entity.Property(e => e.Updated).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.HasMany(e => e.Interactions).WithOne(i => i.Session);
            entity.HasMany(e => e.Messages).WithOne(i => i.Session);
        });

        modelBuilder.Entity<Interaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(d => d.Session).WithMany(p => p.Interactions);
            entity.HasOne(d => d.User).WithMany(p => p.Interactions);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.HasMany(e => e.Interactions).WithOne(i => i.User);
            entity.HasMany(e => e.Messages).WithOne(i => i.User);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Created).IsRequired();
            entity.HasOne<Session>(e => e.Session).WithMany(e => e.Messages);
            entity.HasOne<User>(e => e.User).WithMany(e => e.Messages);
        });
    }
}