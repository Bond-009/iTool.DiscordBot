using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        private static List<string> badWords;

        public static async Task Start()
        {
            Settings.Load();
            if (File.Exists(Settings.Static.SettingsDir + Path.DirectorySeparatorChar + "badwordlist.txt"))
            {
                badWords = File.ReadAllText(Settings.Static.SettingsDir + Path.DirectorySeparatorChar + "badwordlist.txt").Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
            }

            if (string.IsNullOrEmpty(Settings.ApiKeys.DiscordToken))
            {
                Console.WriteLine("No token");
                Console.ReadKey();
                await Quit();
            }
            else
            {
                discordClient = new DiscordSocketClient();

                await discordClient.LoginAsync(TokenType.Bot, Settings.ApiKeys.DiscordToken);
                await discordClient.ConnectAsync();
                Console.WriteLine("Succesfully connected.");

                if (!string.IsNullOrEmpty(Settings.General.Game))
                {
                    await discordClient.SetGameAsync(Settings.General.Game);
                }

                DependencyMap map = new DependencyMap();
                map.Add(discordClient);

                commandHandler = new CommandHandler();
                await commandHandler.Install(map);

                discordClient.Log += Log;
                discordClient.MessageReceived += DiscordClient_MessageReceived;
            }

            while (true)
            {
                string input = Console.ReadLine().ToLower();
                if (input == "quit" || input == "exit" || input == "stop")
                {
                    await Quit();
                }
            }
        }

        private async static Task DiscordClient_MessageReceived(SocketMessage arg)
        {
#if DEBUG
			Console.WriteLine("[" + arg.Timestamp.UtcDateTime + "]" + arg.Author.Username + ": " + arg.Content);
#endif
            if (Settings.General.AntiSwear)
            {
                foreach (string badWord in badWords)
                {
                    if (Regex.Replace(arg.Content.ToLower(), "[^A-Za-z0-9]", "").Contains(badWord))
                    {
                        await arg.DeleteAsync();
                        await arg.Channel.SendMessageAsync(arg.Author.Mention + ", please don't put such things in chat");
                    }
                }
            }
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