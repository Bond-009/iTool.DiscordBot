using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace iTool.DiscordBot
{
    public static class Program
    {
        public static void Main(string[] args) => Start().GetAwaiter().GetResult();

        public static CommandHandler CommandHandler { get; private set; }
        public static Settings Settings { get; private set; }

        private static DiscordSocketClient discordClient;
        private static List<string> badWords;

        public static async Task Start()
        {
            LoadSettings();
            if (File.Exists(Common.SettingsDir + Path.DirectorySeparatorChar + "badwordlist.txt"))
            {
                badWords = File.ReadAllText(Common.SettingsDir + Path.DirectorySeparatorChar + "badwordlist.txt")
                    .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            }

            if (string.IsNullOrEmpty(Settings.DiscordToken))
            {
                Console.WriteLine("No token");
                Console.ReadKey();
                Environment.Exit(0);
            }
            
            discordClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = Settings.AlwaysDownloadUsers,
                LogLevel = Settings.LogLevel,
                MessageCacheSize = Settings.MessageCacheSize
            });
            
            await discordClient.LoginAsync(TokenType.Bot, Settings.DiscordToken);
            await discordClient.ConnectAsync();
            Console.WriteLine("Succesfully connected.");

            if (!string.IsNullOrEmpty(Settings.Game))
            {
                await discordClient.SetGameAsync(Settings.Game);
            }

            DependencyMap map = new DependencyMap();
            map.Add(discordClient);

            CommandHandler = new CommandHandler();
            await CommandHandler.Install(map);

            discordClient.Log += Log;
            discordClient.MessageReceived += DiscordClient_MessageReceived;

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
#if DEBUG
            Console.WriteLine("[" + arg.Timestamp.UtcDateTime.ToString("dd/MM/yyyy HH:mm:ss") + "]" 
                + arg.Author.Username + ": " + arg.Content);
#endif
            if (Settings.AntiSwear && !badWords.IsNullOrEmpty()
                && badWords.Any(Regex.Replace(arg.Content.ToLower(), "[^A-Za-z0-9]", "").Contains))
            {
                await arg.DeleteAsync();
                await arg.Channel.SendMessageAsync(arg.Author.Mention + ", please don't put such things in chat");
            }
        }

        public static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            if (msg.Exception != null) { Console.WriteLine(msg.Exception.ToString()); }
            return Task.CompletedTask;
        }

        public static async Task Quit()
        {
            await discordClient.DisconnectAsync();
            await discordClient.LogoutAsync();
            discordClient.Dispose();
            SaveSettings();
            Environment.Exit(0);
        }

        public static void LoadSettings()
        {
            if (File.Exists(Common.SettingsFile))
            {
                using (FileStream fs = new FileStream(Common.SettingsFile, FileMode.Open))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Settings));
                    Settings = (Settings)ser.Deserialize(fs);
                }
            }
            else
            {
                ResetSettings();
            }
        }

        public static void SaveSettings()
        {
            using (FileStream fs = new FileStream(Common.SettingsFile, FileMode.OpenOrCreate))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                ser.Serialize(fs, Settings);
            }
        }

        public static void ResetSettings()
        {
            if (!Directory.Exists(Common.SettingsDir))
            {
                Directory.CreateDirectory(Common.SettingsDir);
            }
            if (File.Exists(Common.SettingsFile))
            {
                File.Delete(Common.SettingsFile);
                Console.WriteLine("Settings reset.");
            }
            using (FileStream fs = new FileStream(Common.SettingsFile, FileMode.OpenOrCreate))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                ser.Serialize(fs, new Settings());
            }
            LoadSettings();
        }
    }
}
