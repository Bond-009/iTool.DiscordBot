using Discord.WebSocket;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iTool.DiscordBot
{
    public class AntiSwear
    {
        IEnumerable<string> bannedWords;
        public AntiSwear(DiscordSocketClient discordClient)
        {
            bannedWords = Utils.LoadListFromFile(Common.SettingsDir + Path.DirectorySeparatorChar + "banned_words.txt");

            if (!bannedWords.IsNullOrEmpty())
            {
                discordClient.MessageReceived += CheckForBannedWords;
            }
        }

        private async Task CheckForBannedWords(SocketMessage arg)
        {
            if (bannedWords.Any(Regex.Replace(arg.Content.ToLower(), "[^A-Za-z0-9]", "").Contains))
            {
                await arg.DeleteAsync();
                await arg.Channel.SendMessageAsync(arg.Author.Mention + ", please don't put such things in chat");
            }
        }
    }
}
