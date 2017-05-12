using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace iTool.DiscordBot
{
    // TODO: Rework
    public class AudioFileService
    {
        public AudioFileService()
        {
            if (!File.Exists(Common.AudioIndexFile))
            {
                ResetAudioIndex();
            }

            try
            {
                audioFiles = new Deserializer().Deserialize<IEnumerable<AudioFile>>(File.ReadAllText(Common.AudioIndexFile));
            }
            catch (Exception ex)
            {
                Logger.Log(new LogMessage(LogSeverity.Error, nameof(AudioFileService), ex.Message, ex));
            }
        }

        private IEnumerable<AudioFile> audioFiles;

        public string GetSong(string name)
        {
            string path = Path.Combine(Common.AudioDir, audioFiles.FirstOrDefault(x => x.Names.Contains(name))?.FileName);
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
