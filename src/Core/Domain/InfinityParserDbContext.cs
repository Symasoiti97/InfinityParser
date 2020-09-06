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
                .HasIndex(i => new {i.Url, Type = i.ItemType})
                .IsUnique();
            modelBuilder.Entity<SiteDb>()
                .Property(i => i.Url)
                .IsRequired();
            modelBuilder.Entity<SiteDb>()
                .HasIndex(i => i.CreatedDate);
            modelBuilder.Entity<SiteDb>()
                .Property(i => i.ItemType)
                .IsRequired();
            modelBuilder.Entity<SiteDb>()
                .Property(i => i.ItemParentType)
                .IsRequired();

            modelBuilder.Entity<ParserSiteDb>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<ParserSiteDb>()
                .HasOne(i => i.Client)
                .WithMany(i => i.ParserSites)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ParserSiteDb>()
                .HasOne(i => i.Site)
                .WithMany(i => i.ParserSites)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ParserSiteDb>()
                .Property(i => i.Notifications)
                .HasColumnType("jsonb");
            modelBuilder.Entity<ParserSiteDb>()
                .Property(i => i.Notifications)
                .IsRequired();
            modelBuilder.Entity<ParserSiteDb>()
                .Property(i => i.IncludeFilter)
                .HasColumnType("jsonb");
            modelBuilder.Entity<ParserSiteDb>()
                .Property(i => i.ExcludeFilter)
                .HasColumnType("jsonb");
            modelBuilder.Entity<ParserSiteDb>()
                .Property(i => i.IntervalFrom)
                .IsRequired();
            modelBuilder.Entity<ParserSiteDb>()
                .Property(i => i.IntervalTo)
                .IsRequired();
            modelBuilder.Entity<ParserSiteDb>()
                .Property(i => i.IsChildParse)
                .IsRequired();

            modelBuilder.Entity<ItemDb>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<ItemDb>()
                .HasIndex(i => i.Url)
                .IsUnique();
            modelBuilder.Entity<ItemDb>()
                .HasIndex(i => i.CreatedDate);
        }
    }
}