using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace iTool.DiscordBot.Modules
{
    public class CoreModule : ModuleBase<SocketCommandContext>
    {
        private readonly Settings _settings;

        public CoreModule(Settings settings) => _settings = settings;

        [Command("blacklist", RunMode = RunMode.Sync)]
        [Summary("Adds the user to the blacklist")]
        [RequireTrustedUser]
        public async Task Blacklist(params IUser[] users)
        {
            List<IUser> blacklistedUsers = new List<IUser>();

            foreach (IUser user in users)
            {
                if (!_settings.TrustedUsers.Contains(user.Id)
                    && !_settings.BlacklistedUsers.Contains(user.Id)
                    && user.Id != (await Context.Client.GetApplicationInfoAsync().ConfigureAwait(false)).Owner.Id)
                {
                    _settings.BlacklistedUsers.Add(user.Id);
                    blacklistedUsers.Add(user);
                }
            }

            if (blacklistedUsers.Any())
            {
                await ReplyAsync(string.Empty, embed: new EmbedBuilder()
                {
                    Title = "Blacklist",
                    Color = _settings.GetColor(),
                    Description = $"Successfully blacklisted {string.Join(", ", blacklistedUsers)}."
                }.Build()).ConfigureAwait(false);
            }
            else
            {
                await ReplyAsync(string.Empty, embed: new EmbedBuilder()
                {
                    Title = "Blacklist",
                    Color = _settings.GetErrorColor(),
                    Description = $"Failed to blacklist {string.Join(", ", (object[])users)}."
                }.Build()).ConfigureAwait(false);
            }
        }

        [Command("rmblacklist", RunMode = RunMode.Sync)]
        [Summary("Removes the user from the blacklist")]
        [RequireTrustedUser]
        public async Task RmBlacklist(params IUser[] users)
        {
            List<IUser> rmBlacklistedUsers = new List<IUser>();

            foreach (IUser user in users)
            {
                if (_settings.BlacklistedUsers.Contains(user.Id))
                {
                    _settings.BlacklistedUsers.RemoveAll(x => x == user.Id);
                    rmBlacklistedUsers.Add(user);
                }
            }
            if (rmBlacklistedUsers.Count > 0)
            {
                await ReplyAsync(
                    string.Empty,
                    embed: new EmbedBuilder()
                    {
                        Title = "Remove blacklist",
                        Color = _settings.GetColor(),
                        Description = $"Successfully removed blacklist for {string.Join(", ", rmBlacklistedUsers)}."
                    }.Build()).ConfigureAwait(false);
            }
            else
            {
                await ReplyAsync(
                    string.Empty,
                    embed: new EmbedBuilder()
                    {
                        Title = "Remove blacklist",
                        Color = _settings.GetErrorColor(),
                        Description = $"Failed to remove blacklist for {string.Join(", ", (object[])users)}."
                    }.Build()).ConfigureAwait(false);
            }
        }

        [Command("trust", RunMode = RunMode.Sync)]
        [Summary("Adds the user to the list of trusted users")]
        [RequireOwner]
        public async Task Trust(params IUser[] users)
        {
            foreach (IUser user in users)
            {
                _settings.BlacklistedUsers.RemoveAll(x => x == user.Id);
                if (!_settings.TrustedUsers.Contains(user.Id))
                {
                    _settings.TrustedUsers.Add(user.Id);
                }
            }

            await ReplyAsync(string.Empty, embed: new EmbedBuilder()
            {
                Title = "Trust",
                Color = _settings.GetColor(),
                Description = $"Successfully added {string.Join(", ", (object[])users)} to the list of trusted users."
            }.Build()).ConfigureAwait(false);
        }

        [Command("untrust", RunMode = RunMode.Sync)]
        [Summary("Removes the user from the list of trusted users")]
        [RequireOwner]
        public async Task UnTrust(params IUser[] users)
        {
            foreach (IUser user in users)
            {
                if (_settings.TrustedUsers.Contains(user.Id))
                {
                    _settings.TrustedUsers.Remove(user.Id);
                }
            }

            await ReplyAsync(string.Empty, embed: new EmbedBuilder()
            {
                Title = "UnTrust",
                Color = _settings.GetColor(),
                Description = $"Successfully removed {string.Join(", ", (object[])users)} from the list of trusted users."
            }.Build()).ConfigureAwait(false);
        }
    }
}
