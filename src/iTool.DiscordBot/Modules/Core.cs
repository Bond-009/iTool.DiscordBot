using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Core : ModuleBase
    {
        [Command("blacklist")]
        [Summary("Adds the user to the blacklist.")]
        public async Task Blacklist(params IUser[] users)
        {
            if (!Utils.IsTrustedUser(Context.User))
            { return;}

            List<IUser> blacklistedUsers = new List<IUser>();

            foreach (IUser user in users)
            {
                if (!Program.TrustedUsers.Contains(user.Id) && !Program.BlacklistedUsers.Contains(user.Id))
                {
                    Program.BlacklistedUsers.Add(user.Id);
                    blacklistedUsers.Add(user);
                }
            }
            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "Blacklist",
                Color = new Color(3, 144, 255),
                Description = $"Succesfully blacklisted {string.Join(", ", blacklistedUsers)}"
            });
        }
    }
}
