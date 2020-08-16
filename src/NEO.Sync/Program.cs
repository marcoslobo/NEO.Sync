using NEO.Api.Data;
using NEO.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NEO.Api.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.AddEnvironmentVariables();                    
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<SyncWorker>();                    
                });
    }
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
            => services.AddDbContext<NeoContext>();
    }

}
