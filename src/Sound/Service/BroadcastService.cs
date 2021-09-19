using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Hosting;
using Sound.Extension;
using Sound.Manager;

namespace Sound.Service
{
    public class BroadcastService : BackgroundService
    {
        private readonly DiscordClient client;
        private readonly MusicManager manager;

        public BroadcastService(MusicManager manager, DiscordClient client)
        {
            this.manager = manager;
            this.client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var queue = manager.GetQueue();
                while (queue.TryPeek(out var currentMusic))
                {
                    var connection = await client.ConnectToVoiceAsync(currentMusic.Voice);
                    var sink = connection.GetTransmitSink();

                    var position = 0;
                    foreach (var music in queue) // Update all pending musics message
                    {
                        var duration = TimeSpan.FromSeconds(music.Video.Info.LengthSeconds ?? 0);
                        await music.Message.ModifyAsync(new DiscordEmbedBuilder()
                            .WithColor(position == 0 ? DiscordColor.SpringGreen : DiscordColor.CornflowerBlue)
                            .WithTitle($"{music.Video.Title}")
                            .AddField("Author", music.Video.Info.Author, true)
                            .AddField("Duration", duration.ToString(@"m\:ss"), true)
                            .AddField("Requested by", music.Requester.Mention, true)
                            .AddField("Status", position++ == 0 ? "Listening" : $"Waiting (n°{position})")
                            .AddField("Url", music.Url).Build());
                    }

                    await client.UpdateStatusAsync(new DiscordActivity($"{currentMusic.Video.Title}", ActivityType.ListeningTo));
                    using (currentMusic)
                    {
                        await currentMusic.Audio.CopyToSinkAsync(sink, currentMusic.Cts.Token);
                    }
                    await client.UpdateStatusAsync(new DiscordActivity());

                    await currentMusic.Message.DeleteAsync();
                    
                    queue.TryDequeue(out _);

                    if (queue.IsEmpty)
                    {
                        connection.Disconnect();
                    }
                }

                await Task.Delay(100);
            }
        }
    }
}