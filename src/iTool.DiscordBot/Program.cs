using System;
using System.Threading;

namespace iTool.DiscordBot
{
    public static class Program
    {
        static bool running = true;

        public static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            Bot iToolBot = new Bot();
            if (!iToolBot.Start().GetAwaiter().GetResult())
            {
                if (!Console.IsInputRedirected) Console.ReadKey();
                return;
            }

            if (!Console.IsInputRedirected)
            { new Thread(Input).Start(); }

            while (running) ;

            iToolBot.Stop().GetAwaiter().GetResult();

            Environment.Exit(0);
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

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            running = false;
        }
    }
}
