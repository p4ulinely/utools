using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace utools
{
    public class Program
    {

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            
            // migrations (ou descomenta ou faz o migration manual (dotnet ef database update))
            // new DataContext().Database.Migrate();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}