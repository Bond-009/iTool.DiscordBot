using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace iTool.DiscordBot.Modules
{
	public class Default : ModuleBase
	{
		[Command("help")]
		[Summary("Returns all the enabled commands")]
		public async Task Help()
		{
			//TODO: Add help
		}

		[Command("info")]
		public async Task Info()
		{
			IApplication application = await Context.Client.GetApplicationInfoAsync();

			await ReplyAsync(
				Format.Code(Format.Bold("Info") + Environment.NewLine +
				$"- Author: {application.Owner.Username} (ID {application.Owner.Id})" + Environment.NewLine +
				$"- Library: Discord.Net ({DiscordConfig.Version})\n" +
				$"- Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}" + Environment.NewLine +
				$"- Uptime: {GetUptime()}" + Environment.NewLine +
				Environment.NewLine +
				$"{Format.Bold("Stats")}" + Environment.NewLine +
				$"- Heap Size: {GetHeapSize()} MB" + Environment.NewLine +
				$"- Guilds: {(Context.Client as DiscordSocketClient).Guilds.Count}" + Environment.NewLine +
				$"- Channels: {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Channels.Count)}" + Environment.NewLine +
				$"- Users: {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count)}", "md")
			);
		}

		[Command("invite")]
		[Summary("Returns the OAuth2 Invite URL of the bot")]
		public async Task Invite()
		{
			IApplication application = await Context.Client.GetApplicationInfoAsync();
			await ReplyAsync($"A user with `MANAGE_SERVER` can invite me to your server here: <https://discordapp.com/oauth2/authorize?client_id={application.Id}&scope=bot>");
		}

		[Command("leave")]
		[Summary("Instructs the bot to leave this Guild.")]
		[RequireUserPermission(GuildPermission.ManageGuild)]
		public async Task Leave()
		{
			if (Context.Guild == null) { await ReplyAsync("This command can only be ran in a server."); return; }
			await ReplyAsync("Leaving~");
			await Context.Guild.LeaveAsync();
		}

		[Command("say")]
		[Alias("echo")]
		[Summary("Echos the provided input")]
		public async Task Say([Remainder] string input)
		{
			await ReplyAsync(input);
		}

		[Command("setgame")]
		[Summary("Sets the bot's game")]
		public async Task SetGame([Remainder] string input)
		{
			if ((await Context.Client.GetApplicationInfoAsync()).Owner.Id == Context.Message.Author.Id)
			{
				await (Context.Client as DiscordSocketClient).SetGame(input);
			}
		}

		[Command("quit")]
		[Alias("exit", "stop")]
		[Summary("Sets the bot's game")]
		public async Task Quit()
		{
			if ((await Context.Client.GetApplicationInfoAsync()).Owner.Id == Context.Message.Author.Id)
			{
				await Program.Quit();
			}
		}

		private static string GetUptime() => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
		private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
	}
}