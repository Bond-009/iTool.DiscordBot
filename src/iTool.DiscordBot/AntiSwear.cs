using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace iTool.DiscordBot
{
    public class AntiSwear
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly IEnumerable<string> _bannedWords;

        public AntiSwear(DiscordSocketClient discordClient)
        {
            _discordClient = discordClient;
            _bannedWords = Utils.LoadListFromFile(Common.SettingsDir + Path.DirectorySeparatorChar + "banned_words.txt");
        }

        public void AddHandler()
        {
            if (!_bannedWords.IsNullOrEmpty())
            {
                _discordClient.MessageReceived += CheckForBannedWords;
            }
        }

        public void RemoveHandler()
            => _discordClient.MessageReceived -= CheckForBannedWords;

        private async Task CheckForBannedWords(SocketMessage arg)
        {
            if (_bannedWords.Any(Regex.Replace(arg.Content.ToLower(), "[^A-Za-z0-9]", "").Contains))
            {
                await arg.DeleteAsync();
                await arg.Channel.SendMessageAsync(arg.Author.Mention + ", please don't put such things in chat");
            }
        }
    }
}
