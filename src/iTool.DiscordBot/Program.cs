using System;
using System.Threading;

namespace iTool.DiscordBot
{
    public static class DiscordBot
    {
        static bool running = true;

        public static void Main(string[] args)
        {
            Bot iToolBot = new Bot(SettingsManager.LoadSettings());
            if (!iToolBot.Start().GetAwaiter().GetResult())
            { return; }

            if (!Console.IsInputRedirected)
            { new Thread(Input).Start(); }

            while (running) ;

            SettingsManager.SaveSettings(iToolBot.GetSettings());
            iToolBot.Stop().GetAwaiter().GetResult();
        }

        static void Input()
        {
            while (running)
            {
                string input = Console.ReadLine().ToLower();
                switch (input)
                {
                    case "quit":
                    case "exit":
                    case "stop":
                        running = false;
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
    }
}
