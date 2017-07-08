using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace iTool.DiscordBot.Modules
{
    public class DevModule : ModuleBase<SocketCommandContext>
    {
        private readonly Settings _settings;

        public DevModule(Settings settings) => _settings = settings;

        [Command("gc")]
        [Alias("collectgarbage")]
        [Summary("Forces the GC to clean up resources")]
        [RequireTrustedUser]
        public async Task CollectGarbage ()
        {
            GC.Collect();

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "GC",
                Color = _settings.GetColor(),
                Description = "👍"
            });
        }

        [Command("eval", RunMode = RunMode.Async)]
        [Alias("cseval", "csharp", "evaluate")]
        [Summary("Evaluates C# code")]
        [RequireTrustedUser]
        public async Task Eval([Remainder]string input)
        {
            int index1 = input.IndexOf('\n', input.IndexOf("```") + 3) + 1;
            int index2 = input.LastIndexOf("```");

            if (index1 == -1 || index2 == -1)
                throw new ArgumentException("You need to wrap the code into a code block.");

            string code = input.Substring(index1, index2 - index1);

            Task<IUserMessage> msg = ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "Evaluation",
                Color = _settings.GetColor(),
                Description = "Evaluating..."
            });

            try
            {
                ScriptOptions options = ScriptOptions.Default
                    .AddReferences(new[]
                    {
                        typeof(object).GetTypeInfo().Assembly.Location,
                        typeof(Object).GetTypeInfo().Assembly.Location,
                        typeof(Enumerable).GetTypeInfo().Assembly.Location,
                        typeof(DiscordSocketClient).GetTypeInfo().Assembly.Location,
                        typeof(IMessage).GetTypeInfo().Assembly.Location,
                    })
                    .AddImports(new[]
                    {
                        "Discord",
                        "Discord.Commands",
                        "Discord.WebSocket",
                        "System",
                        "System.Linq",
                        "System.Collections",
                        "System.Collections.Generic",
                    });

                object result = await CSharpScript.EvaluateAsync(code, options, globals:
                    new RoslynGlobals()
                    {
                        Client =  Context.Client,
                        Channel = Context.Channel as SocketTextChannel
                    }
                );

                await (await msg).ModifyAsync(x => x.Embed = new EmbedBuilder
                {
                    Title = "Evaluation",
                    Description = result?.ToString() ?? "Success, nothing got returned",
                    Color = _settings.GetColor()
                }.Build());
            }
            catch (Exception ex)
            {
                await (await msg).ModifyAsync(x => x.Embed = new EmbedBuilder
                {
                    Title = "Evaluation Failure",
                    Description = $"**{ex.GetType()}**: {ex.Message}",
                    Color = _settings.GetErrorColor()
                }.Build());
            }
        }

        public class RoslynGlobals
        {
            public DiscordSocketClient Client { get; set; }
            public SocketTextChannel Channel { get; set; }
        }
    }
}
