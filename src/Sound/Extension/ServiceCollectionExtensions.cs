using System;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sound.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDiscordClient(this IServiceCollection services)
        {
            var token = Environment.GetEnvironmentVariable("TOKEN");
            if (token is null)
            {
                throw new InvalidOperationException("Missing TOKEN environment variable");
            }
            
            services.AddSingleton(new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.None
            }));
        }
    }
}