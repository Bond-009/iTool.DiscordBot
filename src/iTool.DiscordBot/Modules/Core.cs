using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Core : ModuleBase
    {
        DependencyMap depMap;
        public Core(DependencyMap map) => this.depMap = map;

        [Command("blacklist")]
        [Summary("Adds the user to the blacklist")]
        [RequireTrustedUser]
        public async Task Blacklist(params IUser[] users)
        {
            List<IUser> blacklistedUsers = new List<IUser>();

            foreach (IUser user in users)
            {
                if (!depMap.Get<Settings>().TrustedUsers.Contains(user.Id)
                    && !depMap.Get<Settings>().BlacklistedUsers.Contains(user.Id)&& user.Id
                    != (await depMap.Get<DiscordSocketClient>().GetApplicationInfoAsync()).Owner.Id)
                {
                    depMap.Get<Settings>().BlacklistedUsers.Add(user.Id);
                    blacklistedUsers.Add(user);
                }
            }
            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "Blacklist",
                Color = new Color((uint)depMap.Get<Settings>().Color),
                Description = $"Succesfully blacklisted {string.Join(", ", blacklistedUsers)}."
            });
        }

        [Command("rmblacklist")]
        [Summary("Removes the user from the blacklist")]
        [RequireTrustedUser]
        public async Task RmBlacklist(params IUser[] users)
        {
            List<IUser> rmBlacklistedUsers = new List<IUser>();

            foreach (IUser user in users)
            {
                if (depMap.Get<Settings>().BlacklistedUsers.Contains(user.Id))
                {
                    depMap.Get<Settings>().BlacklistedUsers.RemoveAll(x => x == user.Id);
                    rmBlacklistedUsers.Add(user);
                }
            }
            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "Remove blacklist",
                Color = new Color((uint)depMap.Get<Settings>().Color),
                Description = $"Succesfully removed blacklist for {string.Join(", ", rmBlacklistedUsers)}."
            });
        }

        [Command("trust")]
        [Summary("Adds the user to the list of trusted users")]
        [RequireOwner]
        public async Task Trust(params IUser[] users)
        {
            foreach (IUser user in users)
            {
                depMap.Get<Settings>().BlacklistedUsers.RemoveAll(x => x == user.Id);
                if (!depMap.Get<Settings>().TrustedUsers.Contains(user.Id))
                {
                    depMap.Get<Settings>().TrustedUsers.Add(user.Id);
                }
            }
            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "Trust",
                Color = new Color((uint)depMap.Get<Settings>().Color),
                Description = $"Succesfully added {string.Join(", ", users.ToList())} to the list of trusted users."
            });
        }
    }
}
