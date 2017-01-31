using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Administration : ModuleBase
    {
        [Command("delmsgs")]
        [Summary("Deletes the messages")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task DelMsgs(int number = 100, params IUser[] users)
        {
            foreach (IUser user in users)
            {
                await Context.Channel.DeleteMessagesAsync((await Context.Channel.GetMessagesAsync(number).Flatten()).Where(Dmsg => Dmsg.Author.Id == user.Id));
            }
        }

        [Command("delmsgs")]
        [Summary("Deletes the messages")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task DelMsgs(params IUser[] users)
        {
            foreach (IUser user in users)
            {
                await Context.Channel.DeleteMessagesAsync((await Context.Channel.GetMessagesAsync().Flatten()).Where(Dmsg => Dmsg.Author.Id == user.Id));
            }
        }

        [Command("delmsgs")]
        [Summary("Deletes the messages")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task DelMsgs(int number = 100)
        {
            await Context.Channel.DeleteMessagesAsync(await Context.Channel.GetMessagesAsync(number).Flatten());
        }

        [Command("kick")]
        [Summary("Kicks the user")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick(params IGuildUser[] users)
        {
            foreach (IGuildUser user in users)
            {
                await user.KickAsync();
            }
        }

        [Command("ban")]
        [Summary("Bans the user")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Ban(params IGuildUser[] users)
        {
            foreach (IUser user in users)
            {
                await Context.Guild.AddBanAsync(user);
            }
        }
    }
}
