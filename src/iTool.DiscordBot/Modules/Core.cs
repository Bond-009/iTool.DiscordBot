using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Core : ModuleBase<SocketCommandContext>
    {
        Settings settings;

        public Core(Settings settings) => this.settings = settings;

        [Command("blacklist")]
        [Summary("Adds the user to the blacklist")]
        [RequireTrustedUser]
        public async Task Blacklist(params IUser[] users)
        {
            List<IUser> blacklistedUsers = new List<IUser>();

            foreach (IUser user in users)
            {
                if (!settings.TrustedUsers.Contains(user.Id)
                    && !settings.BlacklistedUsers.Contains(user.Id) 
                    && user.Id != (await Context.Client.GetApplicationInfoAsync()).Owner.Id)
                {
                    settings.BlacklistedUsers.Add(user.Id);
                    blacklistedUsers.Add(user);
                }
            }
            if (blacklistedUsers.IsNullOrEmpty())
            {
                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = "Blacklist",
                    Color = settings.GetErrorColor(),
                    Description = $"Failed to blacklist {string.Join(", ", users.ToList())}."
                });
            }
            else
            {
                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = "Blacklist",
                    Color = settings.GetColor(),
                    Description = $"Successfully blacklisted {string.Join(", ", blacklistedUsers)}."
                });
            }
        }

        [Command("rmblacklist")]
        [Summary("Removes the user from the blacklist")]
        [RequireTrustedUser]
        public async Task RmBlacklist(params IUser[] users)
        {
            List<IUser> rmBlacklistedUsers = new List<IUser>();

            foreach (IUser user in users)
            {
                if (settings.BlacklistedUsers.Contains(user.Id))
                {
                    settings.BlacklistedUsers.RemoveAll(x => x == user.Id);
                    rmBlacklistedUsers.Add(user);
                }
            }
            if (rmBlacklistedUsers.IsNullOrEmpty())
            {
                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = "Remove blacklist",
                    Color = settings.GetErrorColor(),
                    Description = $"Failed to remove blacklist for {string.Join(", ", users.ToList())}."
                });
            }
            else
            {
                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = "Remove blacklist",
                    Color = settings.GetColor(),
                    Description = $"Successfully removed blacklist for {string.Join(", ", rmBlacklistedUsers)}."
                });
            }
        }

        [Command("trust")]
        [Summary("Adds the user to the list of trusted users")]
        [RequireOwner]
        public async Task Trust(params IUser[] users)
        {
            foreach (IUser user in users)
            {
                settings.BlacklistedUsers.RemoveAll(x => x == user.Id);
                if (!settings.TrustedUsers.Contains(user.Id))
                {
                    settings.TrustedUsers.Add(user.Id);
                }
            }
            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "Trust",
                Color = settings.GetColor(),
                Description = $"Successfully added {string.Join(", ", users.ToList())} to the list of trusted users."
            });
        }
    }
}
