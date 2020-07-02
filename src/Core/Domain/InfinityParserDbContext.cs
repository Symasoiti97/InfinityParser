using Db.Models;
using Db.Models.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Domain
{
    public sealed class InfinityParserDbContext : DbContext
    {
        static InfinityParserDbContext()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<ItemType>();
        }

        public InfinityParserDbContext(DbContextOptions<InfinityParserDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<ItemType>();
            
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
                .HasIndex(i => new {i.Url, Type = i.ItemType})
                .IsUnique();
            modelBuilder.Entity<SiteDb>()
                .Property(i => i.Notifications)
                .HasColumnType("jsonb");
            modelBuilder.Entity<SiteDb>()
                .Property(i => i.Url)
                .IsRequired();
            modelBuilder.Entity<SiteDb>()
                .HasIndex(i => i.CreateDate);
            modelBuilder.Entity<SiteDb>()
                .Property(i => i.Notifications)
                .IsRequired();
            modelBuilder.Entity<SiteDb>()
                .Property(i => i.ItemType)
                .IsRequired();

            modelBuilder.Entity<ItemDb>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<ItemDb>()
                .HasIndex(i => i.Url)
                .IsUnique();
            modelBuilder.Entity<ItemDb>()
                .HasIndex(i => i.CreateDate);
        }
    }
}