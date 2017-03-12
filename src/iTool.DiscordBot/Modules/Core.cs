using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Core : ModuleBase
    {
        [Command("blacklist")]
        [Summary("Adds the user to the blacklist")]
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
                Color = new Color((uint)Program.Settings.Color),
                Description = $"Succesfully blacklisted {string.Join(", ", blacklistedUsers)}."
            });
        }

        [Command("trust")]
        [Summary("Adds the user to the list of trusted users")]
        public async Task Trust(params IUser[] users)
        {
            if (!Utils.IsTrustedUser(Context.User))
            { return;}

            foreach (IUser user in users)
            {
                Program.BlacklistedUsers.RemoveAll(x => x == user.Id);
                if (!Program.TrustedUsers.Contains(user.Id))
                {
                    Program.TrustedUsers.Add(user.Id);
                }
            }
            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "Trust",
                Color = new Color((uint)Program.Settings.Color),
                Description = $"Succesfully added {string.Join(", ", users.ToList())} to the list of trusted users."
            });
        }
    }
}
