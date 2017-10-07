using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord;
using Nett;

namespace iTool.DiscordBot
{
    // TODO: Rework
    public class AudioFileService
    {
		private IEnumerable<AudioFile> _audioFiles;

        public AudioFileService()
        {
            if (!File.Exists(Common.AudioIndexFile))
            {
                ResetAudioIndex();
            }

            try
            {
                _audioFiles = Toml.ReadFile<IEnumerable<AudioFile>>(Common.AudioIndexFile);
            }
            catch (Exception ex)
            {
                Logger.Log(new LogMessage(LogSeverity.Error, nameof(AudioFileService), ex.Message, ex));
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
