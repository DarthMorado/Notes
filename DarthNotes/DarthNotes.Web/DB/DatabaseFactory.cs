using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DarthNotes.DB;

public class DatabaseFactory : IDesignTimeDbContextFactory<Database>
{
    public Database CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<Database>();

        //Get config
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var basePath = Directory.GetCurrentDirectory();

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile($"appsettings.{environment}.json", optional: false)
            .Build();

        //Get Connection string
        string connectionString = config.GetConnectionString("DefaultConnection");

        //Configure
        optionsBuilder.UseSqlServer(connectionString);

        return new Database(optionsBuilder.Options);
    }
}