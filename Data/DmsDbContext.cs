using CQCDMS.Models;
using Microsoft.EntityFrameworkCore;


namespace CQCDMS.Data
{
    public class DmsDbContext : DbContext
    {
        public DmsDbContext() { }
        public DmsDbContext(DbContextOptions<DmsDbContext> options) : base(options) { }

        public DbSet<Document> Documents { get; set; }
        public DbSet<User> Users { get; set; }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //      if (!optionsBuilder.IsConfigured)
        //     {
        //         // Read connection string from appsettings.json
        //         var config = new ConfigurationBuilder()
        //             .SetBasePath(Directory.GetCurrentDirectory())
        //             .AddJsonFile("appsettings.json")
        //             .Build();

        //         var connectionString = config.GetConnectionString("MySqlConnection");
        //         optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        //     }
        // }


    }
}
