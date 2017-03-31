using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class General : ModuleBase
    {
        DependencyMap depMap;
        CommandService cmdService;

        public General(CommandService service, DependencyMap map)
        {
            this.cmdService = service;
            this.depMap = map;
        }

        [Command("help")]
        [Summary("Returns the enabled commands in lists of 25")]
        public async Task Help(int page = 1)
        {
            page -= 1;
            if (page < 0) { return; }

            IEnumerable<CommandInfo> cmds = cmdService.Commands
                                        .GroupBy(x => x.Name)
                                        .Select(y => y.First())
                                        .OrderBy(x => x.Name)
                                        .Skip(page * 25)
                                        .Take(25);

            if (cmds.IsNullOrEmpty())
            { 
                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = "Help",
                    Color = new Color((uint)depMap.Get<Settings>().Color),
                    Description = "No commands found",
                    Url = "https://github.com/Bond-009/iTool.DiscordBot"
                });
                return;
            }

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = "Commands",
                Color = new Color((uint)depMap.Get<Settings>().Color),
                Description = "Returns the enabled commands in lists of 25.",
                Url = "https://github.com/Bond-009/iTool.DiscordBot"
            };

            foreach (CommandInfo cmd in cmds)
            {
               b.AddField(f =>
                {
                    f.Name = cmd.Name;
                    f.Value = cmd.Summary ?? "No summary";
                });
            }
            await (await Context.User.CreateDMChannelAsync()).SendMessageAsync("", embed: b);
        }

        [Command("cmdinfo")]
        [Alias("commandinfo")]
        [Summary("Returns info about the command")]
        public async Task Help(string cmdName)
        {
            CommandInfo cmd = cmdService.Commands
                                .Where(x => x.Name == cmdName.ToLower()
                                || x.Aliases.Contains(cmdName)).FirstOrDefault();

            if (cmd == null)
            {
                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = "Command info",
                    Color = new Color((uint)depMap.Get<Settings>().Color),
                    Description = "No command found",
                    Url = "https://github.com/Bond-009/iTool.DiscordBot"
                });
                return;
            }

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = "Command info",
                Color = new Color((uint)depMap.Get<Settings>().Color),
                Url = "https://github.com/Bond-009/iTool.DiscordBot"
            };
            b.AddField(f =>
            {
                f.Name = "Name";
                f.Value = cmd.Name;
            });

            if (!cmd.Summary.IsNullOrEmpty())
            {
                b.AddField(f =>
                {
                    f.Name = "Summary";
                    f.Value = cmd.Summary ?? "No summary";
                });
            }

            IEnumerable<string> aliases = cmd.Aliases.Where(x => x != cmd.Name);

            if (!aliases.IsNullOrEmpty())
            {
                b.AddField(f =>
                {
                    f.Name = "Aliases";
                    f.Value = string.Join(", ", aliases);
                });
            }

            if (!cmd.Parameters.IsNullOrEmpty())
            {
                b.AddField(f =>
                {
                    f.Name = "Parameters";
                    f.Value = string.Join(", ", cmd.Parameters);
                });
            }

            await ReplyAsync("", embed: b);
        }

        [Command("info")]
        [Summary("Returns info about the bot")]
        public async Task Info()
        {
            IApplication application = await Context.Client.GetApplicationInfoAsync();

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Color = new Color((uint)depMap.Get<Settings>().Color)
            }
            .AddField(f =>
            {
                f.Name = "Info";
                f.Value = $"- Owner: {application.Owner.Username} (ID {application.Owner.Id})" + Environment.NewLine +
                            $"- Library: Discord.Net ({DiscordConfig.Version})\n" +
                            $"- Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}" + Environment.NewLine +
                            $"- Uptime: {Utils.GetUptime().ToString(@"dd\.hh\:mm\:ss")}";
            })
            .AddField(f =>
            {
                f.Name = "Stats";
                f.Value = $"- Heap Size: {Utils.GetHeapSize()} MB" + Environment.NewLine +
                            $"- Guilds: {(Context.Client as DiscordSocketClient).Guilds.Count}" + Environment.NewLine +
                            $"- Channels: {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Channels.Count)}" + Environment.NewLine +
                            $"- Users: {(Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count)}";
            }));
        }

        [Command("invite")]
        [Summary("Returns the OAuth2 Invite URL of the bot")]
        public async Task Invite()
        {
            IApplication application = await Context.Client.GetApplicationInfoAsync();
            await ReplyAsync($"A user with `MANAGE_SERVER` can invite me to your server here: <https://discordapp.com/oauth2/authorize?client_id={application.Id}&scope=bot>");
        }

        [Command("leave")]
        [Summary("Instructs the bot to leave this Guild")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Leave()
        {
            await ReplyAsync("Leaving...");
            await Context.Guild.LeaveAsync();
        }

        [Command("say")]
        [Alias("echo")]
        [Summary("Echos the provided input")]
        public async Task Say([Remainder] string input)
        {
            await ReplyAsync(input);
        }

        [Command("setgame")]
        [Summary("Sets the bot's game")]
        [RequireTrustedUser]
        public async Task SetGame([Remainder] string input)
        {
            depMap.Get<Settings>().Game = input;
            await (Context.Client as DiscordSocketClient).SetGameAsync(input);
            
        }

        // TODO: Allow without parm
        [Command("userinfo")]
        [Summary("Returns info about the user")]
        public async Task UserInfo(IGuildUser user)
        {
            EmbedBuilder b = new EmbedBuilder()
            {
                Color = new Color((uint)depMap.Get<Settings>().Color),
                ThumbnailUrl = user.GetAvatarUrl(ImageFormat.Auto)
            };
            b.AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Username";
                f.Value = user.Username;
            });
            b.AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Discriminator";
                f.Value = user.Discriminator;
            });
            b.AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Id";
                f.Value = user.Id.ToString();
            });
            b.AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Bot";
                f.Value = user.IsBot.ToString();
            });
            b.AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Created at";
                f.Value = user.CreatedAt.UtcDateTime.ToString("dd/MM/yyyy HH:mm:ss");
            });
            if (user.JoinedAt == null)
            {
                b.AddField(f =>
                {
                    f.IsInline = true;
                    f.Name = "Joined at";
                    f.Value = user.JoinedAt.Value.UtcDateTime.ToString("dd/MM/yyyy HH:mm:ss");
                });
            }
            await ReplyAsync("", embed: b);
        }

        //[Command("quit", RunMode = RunMode.Async)]
        //[Alias("exit")]
        //[Summary("Quits the bot")]
        //[RequireTrustedUser]
        //public async Task Quit() => await Program.Quit();
    }
}
