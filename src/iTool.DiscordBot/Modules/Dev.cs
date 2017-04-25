using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Dev : ModuleBase
    {
        DependencyMap depMap;

        public Dev(DependencyMap map) => this.depMap = map;

        [Command("gc")]
        [Alias("collectgarbage")]
        [Summary("Forces the GC to clean up resources")]
        [RequireTrustedUser]
        public async Task GC()
        {
            System.GC.Collect();

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "GC",
                Color = new Color((uint)depMap.Get<Settings>().Color),
                Description = ":thumbsup:"
            });
        }

        [Command("eval")]
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

            IUserMessage msg = await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "Evaluation",
                Color = new Color((uint)depMap.Get<Settings>().Color),
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

                RoslynGlobals global = new RoslynGlobals() {
                    Client = Context.Client as DiscordSocketClient,
                    Channel = Context.Channel as SocketTextChannel
                };

                object result = await CSharpScript.EvaluateAsync(code, options, globals: global);
                await msg.ModifyAsync(x => x.Embed = new EmbedBuilder
                {
                    Title = "Evaluation",
                    Description = result?.ToString() ?? "Success, nothing got returned",
                    Color = new Color((uint)depMap.Get<Settings>().Color)
                }.Build());
            }
            catch (Exception ex)
            {
                await msg.ModifyAsync(x => x.Embed = new EmbedBuilder
                {
                    Title = "Evaluation Failure",
                    Description = $"**{ex.GetType().ToString()}**: {ex.Message}",
                    Color = new Color((uint)depMap.Get<Settings>().ErrorColor)
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
