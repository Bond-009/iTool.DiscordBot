using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace iTool.DiscordBot
{
    public static class TagManager
    {
        public static Tag GetTag(string name, ulong guildID)
        {
            if (!File.Exists(Common.GuildsDir + Path.DirectorySeparatorChar + guildID + Path.DirectorySeparatorChar + "tags.yaml"))
            { return null; }

            try
            {
                return new Deserializer().Deserialize<List<Tag>>(File.ReadAllText(Common.GuildsDir + Path.DirectorySeparatorChar + guildID + Path.DirectorySeparatorChar + "tags.yaml"))
                            .Where(x => x.Title == name.ToLower())
                            .First();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static bool CreateTag(Tag tag, ulong guildID)
        {
            Directory.CreateDirectory(Common.GuildsDir + Path.DirectorySeparatorChar + guildID);

            List<Tag> tags = new List<Tag>();

            if (File.Exists(Common.GuildsDir + Path.DirectorySeparatorChar + guildID + Path.DirectorySeparatorChar + "tags.yaml"))
            {
                tags = new Deserializer().Deserialize<List<Tag>>(File.ReadAllText(Common.GuildsDir + Path.DirectorySeparatorChar + guildID + Path.DirectorySeparatorChar + "tags.yaml"));
            }
            if (!tags.Any(x => x.Title == tag.Title))
            {
                tags.Add(tag);
                File.WriteAllText(Common.GuildsDir + Path.DirectorySeparatorChar + guildID + Path.DirectorySeparatorChar + "tags.yaml",
                new SerializerBuilder().EmitDefaults().Build()
                    .Serialize(tags));
                return true;
            }
            return false;
        }
    }
}
