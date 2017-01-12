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
    static class Program
    {
        public static void Main(string[] args) => Start().GetAwaiter().GetResult();

        private static DiscordSocketClient discordClient;
        private static CommandHandler commandHandler;
        private static List<string> badWords;

        public static Settings settings { get; set; }

        public static async Task Start()
        {
            LoadSettings();
            if (File.Exists(Settings.Static.SettingsDir + Path.DirectorySeparatorChar + "badwordlist.txt"))
            {
                badWords = File.ReadAllText(Settings.Static.SettingsDir + Path.DirectorySeparatorChar + "badwordlist.txt").Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
            }

            if (string.IsNullOrEmpty(settings.DiscordToken))
            {
                Console.WriteLine("No token");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                discordClient = new DiscordSocketClient();

                await discordClient.LoginAsync(TokenType.Bot, settings.DiscordToken);
                await discordClient.ConnectAsync();
                Console.WriteLine("Succesfully connected.");

                if (!string.IsNullOrEmpty(settings.Game))
                {
                    await discordClient.SetGameAsync(settings.Game);
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
            if (settings.AntiSwear)
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
            SaveSettings();
            Environment.Exit(0);
        }

        public static void LoadSettings()
        {
            if (File.Exists(Settings.Static.SettingsFile))
            {
                using (FileStream fs = new FileStream(Settings.Static.SettingsFile, FileMode.Open))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Settings));
                    settings = (Settings)ser.Deserialize(fs);
                }
            }
            else
            {
                ResetSettings();
            }
        }

        public static void SaveSettings()
        {
            using (FileStream fs = new FileStream(Settings.Static.SettingsFile, FileMode.OpenOrCreate))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                ser.Serialize(fs, settings);
            }
        }

        public static void ResetSettings()
        {
            if (!Directory.Exists(Settings.Static.SettingsDir))
            {
                Directory.CreateDirectory(Settings.Static.SettingsDir);
            }
            if (File.Exists(Settings.Static.SettingsFile))
            {
                File.Delete(Settings.Static.SettingsFile);
                Console.WriteLine("Settings reset.");
            }
            using (FileStream fs = new FileStream(Settings.Static.SettingsFile, FileMode.OpenOrCreate))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                ser.Serialize(fs, new Settings());
            }
            LoadSettings();
        }
    }
}