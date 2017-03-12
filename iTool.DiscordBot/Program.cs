using Discord;
using Discord.Commands;
using Discord.WebSocket;
using iTool.DiscordBot.Audio;
using OpenWeather;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iTool.DiscordBot
{
    public static class Program
    {
        public static void Main(string[] args) => Start().GetAwaiter().GetResult();

        public static AudioService AudioService { get; private set; } = new AudioService();
        public static List<ulong> BlacklistedUsers { get; set; }
        public static CommandHandler CommandHandler { get; private set; } = new CommandHandler();
        public static OpenWeatherClient OpenWeatherClient { get; private set; }
        public static Settings Settings { get; set; }

        private static DiscordSocketClient discordClient;
        private static List<string> bannedWords;

        public static async Task Start()
        {
            try
            {
                Settings = SettingsManager.LoadSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                Environment.Exit(1);
            }

            OpenWeatherClient = new OpenWeatherClient(Settings.OpenWeatherMapKey);

            if (File.Exists(Common.SettingsDir + Path.DirectorySeparatorChar + "backlisted_users.txt"))
            {
                bannedWords = File.ReadAllText(Common.SettingsDir + Path.DirectorySeparatorChar + "backlisted_users.txt")
                    .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            }

            if (File.Exists(Common.SettingsDir + Path.DirectorySeparatorChar + "banned_words.txt"))
            {
                bannedWords = File.ReadAllText(Common.SettingsDir + Path.DirectorySeparatorChar + "banned_words.txt")
                    .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            }

            if (!File.Exists(Common.AudioIndexFile)) { AudioManager.ResetAudioIndex(); }

            if (string.IsNullOrEmpty(Settings.DiscordToken))
            {
                Console.WriteLine("No token");
                Console.ReadKey();
                Environment.Exit(0);
            }

            discordClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = Settings.AlwaysDownloadUsers,
                AudioMode = Settings.AudioMode,
                ConnectionTimeout = Settings.ConnectionTimeout,
                DefaultRetryMode = Settings.DefaultRetryMode,
                LogLevel = Settings.LogLevel,
                MessageCacheSize = Settings.MessageCacheSize
            });

            discordClient.Log += Log;
            discordClient.MessageReceived += DiscordClient_MessageReceived;

            await discordClient.LoginAsync(TokenType.Bot, Settings.DiscordToken);
            await discordClient.StartAsync();
            await Task.Delay(1000);
            await Log(new LogMessage(LogSeverity.Critical, "Program", "Succesfully connected as " + discordClient.CurrentUser.ToString()));

            if (!string.IsNullOrEmpty(Settings.Game))
            {
                await discordClient.SetGameAsync(Settings.Game);
            }

            DependencyMap map = new DependencyMap();
            map.Add(discordClient);

            await CommandHandler.Install(map, new CommandServiceConfig()
            {
                CaseSensitiveCommands = Settings.CaseSensitiveCommands,
                DefaultRunMode = Settings.DefaultRunMode
            });

            while (true)
            {
                string input = Console.ReadLine().ToLower();
                switch (input)
                {
                    case "quit":
                    case "exit":
                    case "stop":
                        await Quit();
                        break;
                    case "clear":
                    case "cls":
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine(input + ": command not found");
                        break;
                }
            }
        }

        private async static Task DiscordClient_MessageReceived(SocketMessage arg)
        {
            await Log(new LogMessage(LogSeverity.Verbose, "", arg.Author.Username + ": " + arg.Content));
            
            if (Settings.AntiSwear && !bannedWords.IsNullOrEmpty()
                && bannedWords.Any(Regex.Replace(arg.Content.ToLower(), "[^A-Za-z0-9]", "").Contains))
            {
                await arg.DeleteAsync();
                await arg.Channel.SendMessageAsync(arg.Author.Mention + ", please don't put such things in chat");
            }
        }

        public static Task Log(LogMessage msg)
        {
            if (msg.Severity > Settings.LogLevel)
            { return Task.CompletedTask; }

            Console.WriteLine(msg.ToString());
            if (msg.Exception != null) { Console.WriteLine(msg.Exception.ToString()); }
            return Task.CompletedTask;
        }

        public static async Task Quit()
        {
            await discordClient.StopAsync();
            await discordClient.LogoutAsync();
            discordClient.Dispose();

            OpenWeatherClient.Dispose();

            SettingsManager.SaveSettings(Settings);
            Environment.Exit(0);
        }
    }
}
