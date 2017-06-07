using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord;
using YamlDotNet.Serialization;

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
                CreateAudioIndex();
            }

            try
            {
                _audioFiles = new Deserializer().Deserialize<IEnumerable<AudioFile>>(File.ReadAllText(Common.AudioIndexFile));
            }
            catch (Exception ex)
            {
                Logger.Log(new LogMessage(LogSeverity.Error, nameof(AudioFileService), ex.Message, ex));
            }
        }

        public string GetSong(string name)
        {
            string filename = _audioFiles.FirstOrDefault(x => x.Names.Contains(name))?.FileName;
            if (string.IsNullOrEmpty(filename)) return null;
            string path = Path.Combine(Common.AudioDir, filename);
            if (!File.Exists(path)) return null;
            return path;
        }

        public static void CreateAudioIndex()
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
