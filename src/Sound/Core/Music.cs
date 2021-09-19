using System;
using System.IO;
using System.Threading;
using DSharpPlus.Entities;
using VideoLibrary;

namespace Sound.Core
{
    public class Music : IDisposable
    {
        public DiscordChannel Voice { get; init; }
        public DiscordMessage Message { get; init; }
        public YouTubeVideo Video { get; init; }
        public DiscordMember Requester { get; init; }
        public string Url { get; init; }
        public Stream Audio { get; init; }
        public CancellationTokenSource Cts { get; } = new();
        
        public void Dispose()
        {
            Audio.Dispose();
        }
    }
}