using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace iTool.DiscordBot
{
    // TODO: Rework
    public static class AudioManager
    {
        public static string GetSong(string name)
        {
            if (!File.Exists(Common.AudioIndexFile))
            {
                ResetAudioIndex();

                return null;
            }
            try
            {
                return Common.AudioDir + Path.DirectorySeparatorChar +
                            new Deserializer().Deserialize<List<AudioFile>>(File.ReadAllText(Common.AudioIndexFile))
                                .Where(x => x.Names.Contains(name))
                                .FirstOrDefault()?.FileName;
            }
            catch (Exception ex)
            {
                Logger.Log(new LogMessage(LogSeverity.Error, nameof(AudioManager), ex.Message, ex));
                return null;
            }
        }

        public static void ResetAudioIndex()
        {
            Directory.CreateDirectory(Common.AudioDir);

            List<AudioFile> audioindex = new List<AudioFile>();
            audioindex.Add(new AudioFile()
            {
                FileName = string.Empty,
                Names = new List<string>()
            });

            File.WriteAllText(Common.AudioIndexFile,
                new SerializerBuilder().EmitDefaults().Build()
                    .Serialize(audioindex));
        }
    }
}
