using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext;

namespace Sound.Extension
{
    public static class StreamExtensions
    {
        public static async Task CopyToSinkAsync(this Stream source, VoiceTransmitSink destination, CancellationToken token)
        {
            var bufferLength = destination.SampleLength;
            var buffer = ArrayPool<byte>.Shared.Rent(bufferLength);
            
            try
            {
                int bytesRead;
                while ((bytesRead = await source.ReadAsync(buffer.AsMemory(0, bufferLength), token)) != 0)
                {
                    await destination.WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, bytesRead), token);
                }
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
        }
    }
}