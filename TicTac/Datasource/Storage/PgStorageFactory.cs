using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Datasource.Storage;
    public class PgStorageFactory : IDesignTimeDbContextFactory<PgStorage>
    {
        public PgStorage CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PgStorage>();

            optionsBuilder.UseNpgsql("Host=localhost;Database=test;Username=postgres;Password=postgres");

            return new PgStorage(optionsBuilder.Options);
        }
    }
