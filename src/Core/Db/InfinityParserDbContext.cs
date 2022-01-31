using Db.Models;
using Microsoft.EntityFrameworkCore;

namespace Db
{
    public sealed class InfinityParserDbContext : DbContext
    {
        public InfinityParserDbContext(DbContextOptions<InfinityParserDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserDb>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<UserDb>()
                .HasAlternateKey(i => i.Name);

            modelBuilder.Entity<SiteDb>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<SiteDb>()
                .HasIndex(i => new {i.Url});
            modelBuilder.Entity<SiteDb>()
                .Property(i => i.Url)
                .IsRequired();
            modelBuilder.Entity<SiteDb>()
                .HasIndex(i => i.CreatedDate);
            modelBuilder.Entity<SiteDb>()
                .Property(i => i.ItemParentType)
                .IsRequired();

            modelBuilder.Entity<ParserSiteDb>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<ParserSiteDb>()
                .HasOne(i => i.User)
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