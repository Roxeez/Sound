using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sound
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(x =>
                {
                    x.AddFilter("Microsoft", LogLevel.Warning);
                })
                .ConfigureWebHostDefaults(x =>
                {
                    x.UseStartup<Startup>();
                    x.UseKestrel(s =>
                    {
                        s.ListenAnyIP(5000);
                    });
                })
                .UseConsoleLifetime()
                .Build();

            using (host)
            {
                await host.StartAsync();
                await host.WaitForShutdownAsync();
            }
        }
    }
}