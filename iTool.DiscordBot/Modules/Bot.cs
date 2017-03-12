using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Bot : ModuleBase
    {
        [Command("blacklist")]
        [Summary("Adds the user to the blacklist.")]
        public async Task Blacklist(params IUser[] users)
        {
            if (!Utils.IsTrustedUser(Context.User))
            { return;}
            
            foreach (IUser user in users)
            {
                if (!Program.TrustedUsers.Contains(user.Id) && !Program.BlacklistedUsers.Contains(user.Id))
                {
                    Program.BlacklistedUsers.Add(user.Id);
                }
            }
            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "Blacklist",
                Color = new Color(3, 144, 255),
                Description = ":thumbsup:"
            });
        }
    }
}
