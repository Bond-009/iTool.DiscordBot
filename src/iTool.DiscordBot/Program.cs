using System;
using System.Threading;
using System.Threading.Tasks;

namespace iTool.DiscordBot
{
    public static class Program
    {
        private static readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public static async Task<int> Main(string[] args)
        {
            Console.CancelKeyPress += cancelKeyPress;

            Bot iToolBot = new Bot();
            if (!await iToolBot.Start())
            {
                if (!Console.IsInputRedirected) Console.ReadKey();
                return 1;
            }

            if (!Console.IsInputRedirected)
            {
                Task t = Task.Run(() => handleInput(), _tokenSource.Token);
            }

            await Task.Delay(-1, _tokenSource.Token).ContinueWith(x => {});

            await iToolBot.Stop();

            return 0;
        }

        private static void handleInput()
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                string input = Console.ReadLine().ToLower();
                switch (input)
                {
                    case "quit":
                    case "exit":
                    case "stop":
                        _tokenSource.Cancel();
                        break;
                    case "clear":
                    case "cls":
                        Console.Clear();
                        break;
                    case "":
                        break;
                    default:
                        Console.WriteLine(input + ": command not found");
                        break;
                }
            }
        }

        private static void cancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _tokenSource.Cancel();
        }
    }
}
