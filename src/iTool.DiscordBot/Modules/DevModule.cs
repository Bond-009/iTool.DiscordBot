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
        public Task CollectGarbage()
        {
            GC.Collect();

            return ReplyAsync(string.Empty, embed: new EmbedBuilder()
            {
                Title = "GC",
                Color = _settings.GetColor(),
                Description = "👍"
            }.Build());
        }

        [Command("eval", RunMode = RunMode.Async)]
        [Alias("cseval", "csharp", "evaluate")]
        [Summary("Evaluates C# code")]
        [RequireTrustedUser]
        public async Task Eval([Remainder] string input)
        {
            if (!TryGetCode(input, out string code))
            {
                await ReplyAsync(string.Empty, embed: new EmbedBuilder()
                {
                    Title = "Error",
                    Color = _settings.GetErrorColor(),
                    Description = "Syntax Error"
                }.Build());
                return;
            }

            Task<IUserMessage> msg = ReplyAsync(
                string.Empty,
                embed: new EmbedBuilder()
                {
                    Title = "Evaluation",
                    Color = _settings.GetColor(),
                    Description = "Evaluating..."
                }.Build());

            object result = null;
            try
            {
                ScriptOptions options = ScriptOptions.Default
                    .AddReferences(new[]
                    {
                        typeof(object).GetTypeInfo().Assembly.Location,
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
                        "System.Text",
                        "System.Collections",
                        "System.Collections.Generic",
                        "System.Globalization",
                        "System.Threading",
                        "System.Threading.Tasks"
                    });

                result = await CSharpScript.EvaluateAsync(
                    code,
                    options,
                    globals: new RoslynGlobals()
                    {
                        Client = Context.Client,
                        Channel = Context.Channel as SocketTextChannel
                    }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await (await msg.ConfigureAwait(false)).ModifyAsync(
                    x => x.Embed = new EmbedBuilder
                    {
                        Title = "Evaluation Failure",
                        Description = $"**{ex.GetType()}**: {ex.Message}",
                        Color = _settings.GetErrorColor()
                    }.Build()).ConfigureAwait(false);

                return;
            }

            await (await msg.ConfigureAwait(false)).ModifyAsync(
                x => x.Embed = new EmbedBuilder
                {
                    Title = "Evaluation",
                    Description = result?.ToString() ?? "Success, nothing got returned",
                    Color = _settings.GetColor()
                }.Build()).ConfigureAwait(false);
        }

        internal static bool TryGetCode(string input, out string code)
        {
            code = string.Empty;

            int index1 = input.IndexOf("```");
            int index2 = -1;
            if (index1 == -1)
            {
                // Check for inline code block
                index1 = input.IndexOf('`');
                if (index1 == -1)
                {
                    // No code block found, return plain text
                    code = input;
                    return true;
                }

                index2 = input.LastIndexOf('`');
                if (index2 == index1)
                {
                    // No closing backtick
                    return false;
                }

                index1++; // Ignore the backtick itself
            }
            else
            {
                // Check for code block
                index2 = input.LastIndexOf("```");
                if (index1 == index2)
                {
                    // No closing backticks
                    return false;
                }

                index1 = input.IndexOf('\n', index1 + 3) + 1;
            }

            code = input[index1..index2].TrimEnd();
            return code.Length != 0;
        }

        public class RoslynGlobals
        {
            public DiscordSocketClient Client { get; set; }
            public SocketTextChannel Channel { get; set; }
        }
    }
}
