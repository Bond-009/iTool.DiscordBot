using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace iTool.DiscordBot.Modules
{
    public class AdministrationModule : ModuleBase
    {
        [Command("delmsgs")]
        [Alias("purge", "clean")]
        [Summary("Deletes the messages")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        public async Task DeleteMessages(int number = 100)
            => await (Context.Channel as ITextChannel).DeleteMessagesAsync(
                (await Context.Channel.GetMessagesAsync(number).FlattenAsync().ConfigureAwait(false))
                    .Where(x => DateTimeOffset.UtcNow - x.CreatedAt < TimeSpan.FromDays(14))).ConfigureAwait(false);

        [Command("delmsgs")]
        [Alias("purge", "clean")]
        [Summary("Deletes the messages")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        public async Task DeleteMessages(params IUser[] users)
            => await (Context.Channel as ITextChannel).DeleteMessagesAsync(
                (await Context.Channel.GetMessagesAsync().FlattenAsync().ConfigureAwait(false))
                    .Where(x => users.Select(y => y.Id).Contains(x.Author.Id)
                        && DateTimeOffset.UtcNow - x.CreatedAt < TimeSpan.FromDays(14))).ConfigureAwait(false);

        [Command("kick")]
        [Summary("Kicks the user")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick(params IGuildUser[] users)
        {
            foreach (IGuildUser user in users)
            {
                await user.KickAsync().ConfigureAwait(false);
            }
        }

        [Command("ban")]
        [Summary("Bans the user")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Ban(params IUser[] users)
        {
            foreach (IUser user in users)
            {
                await Context.Guild.AddBanAsync(user).ConfigureAwait(false);
            }
        }
    }
}
