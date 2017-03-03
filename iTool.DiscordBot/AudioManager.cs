using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

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
            using (FileStream fs = new FileStream(Common.AudioIndexFile, FileMode.Open))
            {
                XmlSerializer ser = new XmlSerializer(typeof(AudioIndex));
                return Common.AudioDir + Path.DirectorySeparatorChar + ((AudioIndex)ser.Deserialize(fs))
                                                                            .AudioFiles
                                                                            .Where(x => x.Names.Contains(name))
                                                                            .First().FileName;
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
            using (FileStream fs = new FileStream(Common.AudioIndexFile, FileMode.OpenOrCreate))
            {
                XmlSerializer ser = new XmlSerializer(typeof(AudioIndex));
                AudioIndex index = new AudioIndex();
                index.AudioFiles.Add(new AudioFile());
                ser.Serialize(fs, index);
            }
        }
    }
}
