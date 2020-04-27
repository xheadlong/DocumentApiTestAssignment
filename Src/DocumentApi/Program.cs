using System.Threading.Tasks;
using DocumentApi.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DocumentApi
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args)
                .Build();

            await host.InitAsync();
            await host.RunAsync();
        }

        public static Task InitAsync(this IHost host) 
        {
            IStartupInitializer initializer = host.Services.GetRequiredService<IStartupInitializer>();
            return initializer.InitAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    })
                .ConfigureLogging(builder => 
                    builder.AddConsole());
    }
}
