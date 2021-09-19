using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Sound.Manager;
using VideoLibrary;
using Xabe.FFmpeg;

namespace Sound.Command
{
    public class UserCommands : BaseCommandModule
    {
        private readonly MusicManager manager;
        private readonly ILogger<UserCommands>
        public UserCommands(MusicManager manager)
        {
            this.manager = manager;
        }

        [Command("play")]
        public async Task Play(CommandContext ctx, [RemainingText] string url)
        {
            var video = await YouTube.Default.GetVideoAsync(url);

            var duration = TimeSpan.FromSeconds(video.Info.LengthSeconds ?? 0);
            var message = await ctx.Channel.SendMessageAsync(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Red)
                .WithTitle($"{video.Title}")
                .AddField("Author", video.Info.Author, true)
                .AddField("Duration", duration.ToString(@"m\:ss"), true)
                .AddField("Requested by", ctx.Member.DisplayName, true)
                .AddField("Status", "Downloading...")
                .AddField("Url", url));

            var mp3 = Path.GetTempFileName();
            var mp4 = Path.GetTempFileName();
            
            using(var file = File.Create(mp4))
            using (var stream = await video.StreamAsync())
            {
                await stream.CopyToAsync(file);
            }

            await message.ModifyAsync(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Orange)
                .WithTitle($"{video.Title}")
                .AddField("Author", video.Info.Author, true)
                .AddField("Duration", duration.ToString(@"m\:ss"), true)
                .AddField("Requested by", ctx.Member.DisplayName, true)
                .AddField("Status", "Extracting...")
                .AddField("Url", url).Build());
            
            var media = await FFmpeg.GetMediaInfo(mp4);
            var audio = media.AudioStreams.OrderByDescending(x => x.Bitrate).First();
            var conversion = new Conversion()
                .AddStream(audio)
                .SetAudioBitrate(audio.Bitrate)
                .SetOutputFormat(Format.s16le)
                .UseMultiThread(true)
                .SetOverwriteOutput(true)
                .SetOutput(mp3);

            await conversion.Start();
            
            var bytes = await File.ReadAllBytesAsync(mp3);
            
            File.Delete(mp4);
            File.Delete(mp3);
            
            await message.ModifyAsync(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Green)
                .WithTitle($"{video.Title}")
                .AddField("Author", video.Info.Author, true)
                .AddField("Duration", duration.ToString(@"m\:ss"), true)
                .AddField("Requested by", ctx.Member.DisplayName, true)
                .AddField("Position", "Ready")
                .AddField("Url", url).Build());
        }
    }
}