using System;
using System.Threading;

namespace iTool.DiscordBot
{
    public static class DiscordBot
    {
        public static void Main(string[] args)
        {
            Bot iToolBot = new Bot(SettingsManager.LoadSettings());
            if (!iToolBot.Start().GetAwaiter().GetResult())
            { return; }

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            if (!Console.IsInputRedirected)
            { new Thread(() => Input(tokenSource)).Start(); }

            while (!tokenSource.Token.IsCancellationRequested) ;

            SettingsManager.SaveSettings(iToolBot.GetSettings());
            iToolBot.Stop().GetAwaiter().GetResult();
        }

        private static void Input(CancellationTokenSource tokenSource)
        {
            while (!tokenSource.Token.IsCancellationRequested)
            {
                string input = Console.ReadLine().ToLower();
                switch (input)
                {
                    case "quit":
                    case "exit":
                    case "stop":
                        tokenSource.Cancel();
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
