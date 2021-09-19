using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sound.Extension;
using Sound.Manager;
using Sound.Service;

namespace Sound
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDiscordClient();

            services.AddSingleton<MusicManager>();

            services.AddHostedService<BotService>();
            services.AddHostedService<BroadcastService>();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}