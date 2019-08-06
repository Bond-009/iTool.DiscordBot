using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nett;
using Microsoft.Extensions.Logging;

namespace iTool.DiscordBot
{
    // TODO: Rework
    public class AudioFileService
    {
		private IEnumerable<AudioFile> _audioFiles;
        private readonly ILogger _logger;

        public AudioFileService(ILogger<AudioFileService> logger)
        {
            _logger = logger;
            LoadSongs();
        }

        public void LoadSongs()
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
                _logger.LogError(ex, "Error reading audio index file");
            }
        }

        public string GetSong(string name)
        {
            string filename = _audioFiles.FirstOrDefault(x => x.Names.Contains(name))?.FileName;
            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }

            string path = Path.Combine(Common.AudioDir, filename);
            if (!File.Exists(path))
            {
                return null;
            }

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
