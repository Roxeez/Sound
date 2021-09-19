using System.Collections.Concurrent;
using Sound.Core;

namespace Sound.Manager
{

    public class MusicManager
    {
        private readonly ConcurrentQueue<Music> musics = new();
        
        public int AddMusicToQueue(Music music)
        {
            musics.Enqueue(music);
            return musics.Count;
        }

        public ConcurrentQueue<Music> GetQueue()
        {
            return musics;
        }
    }
}