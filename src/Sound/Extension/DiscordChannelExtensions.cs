using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

namespace Sound.Extension
{
    public static class DiscordChannelExtensions
    {
        public static async Task<VoiceNextConnection> ConnectToVoiceAsync(this DiscordClient client, DiscordChannel channel)
        {
            var connection = client.GetVoiceNext().GetConnection(channel.Guild);
            if (connection is null)
            {
                connection = await channel.ConnectAsync();
            }
            else if (connection.TargetChannel != channel)
            {
                connection.Disconnect();
                connection = await channel.ConnectAsync();
            }
            
            return connection;
        }
    }
}