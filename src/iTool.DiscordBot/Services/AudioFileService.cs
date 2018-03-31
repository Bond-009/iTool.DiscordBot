using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nett;
using Serilog;

namespace iTool.DiscordBot
{
    // TODO: Rework
    public class AudioFileService
    {
		private IEnumerable<AudioFile> _audioFiles;
        private readonly ILogger _logger;

        public AudioFileService(ILogger logger)
        {
            _logger = logger;
            loadSongs();
        }
    

        public void loadSongs()
        {
            if (!File.Exists(Common.AudioIndexFile))
            {
                ResetAudioIndex();
            }

            try
            {
                _audioFiles = Toml.ReadFile<List<AudioFile>>(Common.AudioIndexFile); // Temporary workaround
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(AudioFileService)}: {ex.Message}");
            }
        }

        public string GetSong(string name)
        {
            string filename = _audioFiles.FirstOrDefault(x => x.Names.Contains(name))?.FileName;
            if (filename.IsNullOrEmpty()) return null;
            string path = Path.Combine(Common.AudioDir, filename);
            if (!File.Exists(path)) return null;
            return path;
        }

        public static void ResetAudioIndex()
        {
            Directory.CreateDirectory(Common.AudioDir);

            Toml.WriteFile(new AudioFile[]
                        {
                            new AudioFile()
                            {
                                FileName = string.Empty,
                                Names = new string[] { string.Empty }
                            }
                        }
                , Common.AudioIndexFile);
        }
    }
}
