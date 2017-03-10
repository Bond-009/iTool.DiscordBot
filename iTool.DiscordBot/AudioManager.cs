using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace iTool.DiscordBot
{
    public static class AudioManager
    {
        public static string GetSong(string name)
        {
            if (!File.Exists(Common.AudioIndexFile))
            {
                ResetAudioIndex();
            }
            try
            {
                return Common.AudioDir + Path.DirectorySeparatorChar +
                            new Deserializer().Deserialize<List<AudioFile>>(File.ReadAllText(Common.AudioIndexFile))
                                .Where(x => x.Names.Contains(name))
                                .First().FileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static void ResetAudioIndex()
        {
            if (!Directory.Exists(Common.AudioDir))
            {
                Directory.CreateDirectory(Common.AudioDir);
            }
            if (File.Exists(Common.AudioIndexFile))
            {
                File.Delete(Common.AudioIndexFile);
                Console.WriteLine("Audio files reset.");
            }

            List<AudioFile> audioindex = new List<AudioFile>();
            audioindex.Add(new AudioFile() {
                Names = {string.Empty},
                FileName = string.Empty
            });

            File.WriteAllText(Common.AudioIndexFile,
                new SerializerBuilder().EmitDefaults().Build()
                    .Serialize(audioindex));
        }
    }
}
