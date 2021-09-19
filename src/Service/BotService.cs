using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sound.Command;
using Sound.Manager;

namespace Sound.Service
{
    public class BotService : IHostedService
    {
        private readonly DiscordClient client;
        private readonly ILogger<BotService> logger;
        private readonly IServiceProvider services;
        private readonly MusicManager musicManager;

        public BotService(DiscordClient client, ILogger<BotService> logger, IServiceProvider services, MusicManager musicManager)
        {
            this.client = client;
            this.logger = logger;
            this.services = services;
            this.musicManager = musicManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            client.UseVoiceNext();
            var commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                Services = services,
                StringPrefixes = new[] {"!"},
                EnableDms = false
            });
            
            commands.RegisterCommands<MusicCommandModule>();
            
            await client.ConnectAsync();
            
            logger.LogInformation("Discord client successfully connected.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var queue = musicManager.GetQueue();
            while (queue.TryDequeue(out var music))
            {
                music.Cts.Cancel();
                await music.Message.DeleteAsync();
            }
            await client.DisconnectAsync();
            
            logger.LogInformation("Discord client successfully disconnected.");
        }
    }
}