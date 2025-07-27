using Merit_Tracker.Converters;
using Merit_Tracker.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace Merit_Tracker.Database
{
    // This is the database context which represents the whole database and its related tables, etc.
    public class AppDatabaseContext : DbContext
    {
        public AppDatabaseContext(DbContextOptions<AppDatabaseContext> options) : base(options)
        { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<MeritModel> Merits { get; set; }
        public DbSet<DatabaseModel> Databases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed();
            modelBuilder.UseSerialColumns();

            modelBuilder.Entity<MeritModel>().
                Property(m => m.DateOfIssue).
                HasColumnType("timestamptz").
                HasConversion<DateTimeUtcConverter>();
        }
    }

    public static class ModelBuilderExtensions
    {
        public static ModelBuilder Seed(this ModelBuilder builder)
        {

            return builder;

        }
    }
}
