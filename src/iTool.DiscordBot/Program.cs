using System;
using System.Threading;
using System.Threading.Tasks;

namespace iTool.DiscordBot
{
    public static class Program
    {
        static CancellationTokenSource tokenSource = new CancellationTokenSource();

        public static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();
        public static async Task MainAsync(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            Bot iToolBot = new Bot();
            if (!await iToolBot.Start())
            {
                if (!Console.IsInputRedirected) Console.ReadKey();
                return;
            }

            if (!Console.IsInputRedirected)
            { new Thread(Input).Start(); }

            await Task.Delay(-1, tokenSource.Token).ContinueWith(tsk => { });

            await iToolBot.Stop();

            Environment.Exit(0);
        }

        static void Input()
        {
            while (!tokenSource.IsCancellationRequested)
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

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            tokenSource.Cancel();
        }
    }
}
