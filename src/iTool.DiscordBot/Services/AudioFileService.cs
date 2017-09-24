using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord;
using Serilog;
using YamlDotNet.Serialization;

namespace iTool.DiscordBot
{
    // TODO: Rework
    public class AudioFileService
    {
		private IEnumerable<AudioFile> _audioFiles;
        private ILogger _logger = Log.ForContext<AudioFileService>();

        public AudioFileService()
        {
            if (!File.Exists(Common.AudioIndexFile))
            {
                ResetAudioIndex();
            }

            try
            {
                _audioFiles = new Deserializer().Deserialize<IEnumerable<AudioFile>>(File.ReadAllText(Common.AudioIndexFile));
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, ex.Message);
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

            File.WriteAllText(Common.AudioIndexFile,
                new SerializerBuilder().EmitDefaults().Build()
                    .Serialize(new AudioFile[]
                    {
                        new AudioFile()
                        {
                            FileName = string.Empty,
                            Names = new string[] { string.Empty }
                        }
                    }));
        }
    }
}
