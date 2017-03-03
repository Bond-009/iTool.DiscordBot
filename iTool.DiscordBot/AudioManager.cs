using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
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
                Deserializer des = new Deserializer();
                return Common.AudioDir + Path.DirectorySeparatorChar +
                            ((AudioIndex)des.Deserialize(File.ReadAllText(Common.AudioIndexFile), typeof(AudioIndex)))
                                .AudioFiles
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

            AudioIndex index = new AudioIndex();
            AudioFile audio = new AudioFile();
            audio.Names.Add("");
            index.AudioFiles.Add(audio);
            File.WriteAllText(Common.AudioIndexFile, new Serializer().Serialize(index));
        }
    }
}
