using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<WebApisContext>
    {
        public WebApisContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("C:\\D-Drive\\project-draft\\Backend\\WebAPIs\\WebAPIs\\appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<WebApisContext>();

            var connectionString = configuration.GetConnectionString("WebAPIsContext");

            builder.UseSqlServer(connectionString);

            return new WebApisContext(builder.Options);
        }
    }
}
