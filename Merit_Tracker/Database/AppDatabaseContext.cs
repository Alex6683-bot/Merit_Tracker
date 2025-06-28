using Merit_Tracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Merit_Tracker.Database
{
    public class AppDatabaseContext : DbContext
    {
        public AppDatabaseContext(DbContextOptions<AppDatabaseContext> options) : base(options)
        { }

        public DbSet<UserModel> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed();
            modelBuilder.UseSerialColumns();
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
