using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NEO.Api.Data
{
    class EvoluaContextFactory : IDesignTimeDbContextFactory<EvoLuaContext>
    {
        public EvoLuaContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EvoLuaContext>();
            optionsBuilder.UseNpgsql("User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=evolua;Pooling=true");

            return new EvoLuaContext(optionsBuilder.Options);
        }
    }
}
