using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public sealed class InfinityParserDbContext : DbContext
    {
        public InfinityParserDbContext(DbContextOptions<InfinityParserDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<ClientDb> Clients { get; set; }
        public DbSet<ItemDb> Items { get; set; }
        public DbSet<SiteDb> Sites { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientDb>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<ClientDb>()
                .HasAlternateKey(i => i.Name);

            modelBuilder.Entity<SiteDb>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<SiteDb>()
                .HasOne(i => i.Client)
                .WithMany(i => i.Sites)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SiteDb>()
                .HasIndex(i => new {i.Url, i.Type});

            modelBuilder.Entity<ItemDb>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<ItemDb>()
                .HasIndex(i => i.Url);
        }
    }
}