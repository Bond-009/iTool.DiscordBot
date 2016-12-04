using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace iTool.DiscordBot.Modules
{
	public class Administration : ModuleBase
	{
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
		public async Task DelMsgs()
		{
			await Context.Channel.DeleteMessagesAsync(await Context.Channel.GetMessagesAsync().Flatten());
		}

		[Command("kick")]
		[Summary("Kick the user")]
		[RequireUserPermission(GuildPermission.KickMembers)]
		public async Task Kick(params IUser[] users)
		{
			foreach (IUser user in users)
			{
				foreach (IGuildUser guildUser in (await Context.Guild.GetUsersAsync()).Where(t => t.Id == user.Id))
				{
					await guildUser.KickAsync();
				}
			}
		}

		[Command("ban")]
		[Summary("Bans the user")]
		[RequireUserPermission(GuildPermission.BanMembers)]
		public async Task Ban(params IUser[] users)
		{
			foreach (IUser user in users)
			{
				foreach (IGuildUser guildUser in (await Context.Guild.GetUsersAsync()).Where(t => t.Id == user.Id))
				{
					await Context.Guild.AddBanAsync(guildUser);
				}
			}
		}
	}
}