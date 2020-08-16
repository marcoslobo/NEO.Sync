using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace NEO.Api.Data
{
    class NeoContextFactory : IDesignTimeDbContextFactory<NeoContext>
    {
        public NeoContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("");

            var optionsBuilder = new DbContextOptionsBuilder<NeoContext>();
            optionsBuilder.UseNpgsql("User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=evolua;Pooling=true");

            return new NeoContext(optionsBuilder.Options);
        }
    }
}
