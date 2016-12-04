using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using iTool.DiscordBot.Prefrences;

namespace iTool.DiscordBot
{
	static class Program
	{
		public static void Main(string[] args) => Start().GetAwaiter().GetResult();

		private static DiscordSocketClient discordClient;
		private static CommandHandler commandHandler;

		public static async Task Start()
		{
			Settings.Load();

			discordClient = new DiscordSocketClient();

			//TODO: check if Settings.ApiKeys.DiscordToken is empty or null
			if (string.IsNullOrEmpty(Settings.ApiKeys.DiscordToken))
			{
				Console.ReadKey();
				await Quit();
			}
			else
			{
				await discordClient.LoginAsync(TokenType.Bot, Settings.ApiKeys.DiscordToken);
				await discordClient.ConnectAsync();
				Console.WriteLine("Succesfully connected.");

				if (!string.IsNullOrEmpty(Settings.General.Game))
				{
					await discordClient.SetGame(Settings.General.Game);
				}

				DependencyMap map = new DependencyMap();
				map.Add(discordClient);

				commandHandler = new CommandHandler();
				await commandHandler.Install(map);
			}

			discordClient.Log += Log;
			discordClient.MessageReceived += DiscordClient_MessageReceived1;

			while (true)
			{
				string input = Console.ReadLine().ToLower();
				if (input == "quit" || input == "exit" || input == "stop" )
				{
					await Quit();
				}
			}
		}

		private static Task DiscordClient_MessageReceived1(SocketMessage arg)
		{
			Console.WriteLine("[" + arg.Timestamp.UtcDateTime + "]" + arg.Author.Username + ": " + arg.Content);
			/*
			foreach (string badword in badwords)
			{
				string nospecialchar = Regex.Replace(e.Message.Text.ToLower(), "[^A-Za-z0-9]", "");
				if (nospecialchar.Contains(badword))
				{
					await e.Message.Delete();
					await e.Channel.SendMessage(e.User.Mention + ", please don't put such things in chat");
				}
			}*/
			return Task.CompletedTask;
		}

		private static Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		public static async Task Quit()
		{
			await discordClient.DisconnectAsync();
			await discordClient.LogoutAsync();
			discordClient.Dispose();
			Settings.Save();
			Environment.Exit(0);
		}
	}
}