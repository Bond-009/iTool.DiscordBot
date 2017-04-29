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
    public class General : ModuleBase<SocketCommandContext>
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
        [Alias("commandinfo", "cmdinformation", "commandinformation")]
        [Summary("Returns info about the command")]
        public async Task Help(string cmdName)
        {
            CommandInfo cmd = cmdService.Commands
                                .Where(x => x.Aliases.Contains(cmdName)).FirstOrDefault();

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
            }
            .AddField(f =>
            {
                f.Name = "Name";
                f.Value = cmd.Name;
            });

            if (!cmd.Summary.IsNullOrEmpty())
            {
                b.AddField(f =>
                {
                    f.Name = "Summary";
                    f.Value = cmd.Summary;
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
        [Alias("information")]
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
                f.Value = $"- Owner: {application.Owner.ToString()} (ID {application.Owner.Id})" + Environment.NewLine +
                            $"- Library: Discord.Net ({DiscordConfig.Version})\n" +
                            $"- Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}" + Environment.NewLine +
                            $"- Uptime: {Utils.GetUptime().ToString(@"dd\.hh\:mm\:ss")}";
            })
            .AddField(f =>
            {
                f.Name = "Stats";
                f.Value = $"- Heap Size: {Utils.GetHeapSize()} MB" + Environment.NewLine +
                            $"- Guilds: {Context.Client.Guilds.Count}" + Environment.NewLine +
                            $"- Channels: {Context.Client.Guilds.Sum(g => g.Channels.Count)}" + Environment.NewLine +
                            $"- Users: {Context.Client.Guilds.Sum(g => g.Users.Count)}";
            }));
        }

        [Command("invite")]
        [Summary("Returns the OAuth2 Invite URL of the bot")]
        public async Task Invite()
            => await ReplyAsync("A user with `MANAGE_SERVER` can invite me to your server here: " +
                $"<https://discordapp.com/oauth2/authorize?client_id={(await Context.Client.GetApplicationInfoAsync()).Id}&scope=bot>");

        [Command("leave")]
        [Summary("Instructs the bot to leave this Guild")]
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
            => await ReplyAsync(input);

        [Command("setgame")]
        [Summary("Sets the bots game")]
        [RequireTrustedUser]
        public async Task SetGame([Remainder] string input)
        {
            depMap.Get<Settings>().Game = input;
            await Context.Client.SetGameAsync(input);
        }

        [Command("ping")]
        [Summary("Gets the estimated round-trip latency, in milliseconds, to the gateway server")]
        public async Task Ping()
            => await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "Ping",
                Color = new Color((uint)depMap.Get<Settings>().Color),
                Description = $"Latency: {Context.Client.Latency}ms"
            });

        [Command("userinfo")]
        [Summary("Returns info about the user")]
        public async Task UserInfo(SocketUser user = null)
        {
            if (user == null)
            {
                user = Context.User;
            }

            SocketGuildUser gUser = null;
            if (user is SocketGuildUser tGUser)
            {
                gUser = tGUser;
            }

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = $"Info about {user.ToString()}",
                Color = new Color((uint)depMap.Get<Settings>().Color),
                ThumbnailUrl = user.GetAvatarUrl(ImageFormat.Auto)
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Username";
                f.Value = user.Username;
            });
            if (gUser != null && gUser.Nickname != null)
            {
                b.AddField(f =>
                {
                    f.IsInline = true;
                    f.Name = "Nickname";
                    f.Value = gUser.Nickname;
                });
            }
            b.AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Discriminator";
                f.Value = user.Discriminator;
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Id";
                f.Value = user.Id.ToString();
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Status";
                f.Value = user.Status.ToString();
            });
            if (gUser != null)
            {
                b.AddField(f =>
                {
                    f.IsInline = true;
                    f.Name = "Voice status";
                    f.Value = $"- Deafened: {gUser.IsDeafened.ToString()}" + Environment.NewLine +
                        $"- Musted: {gUser.IsMuted.ToString()}" + Environment.NewLine +
                        $"- SelfDeafened: {gUser.IsSelfDeafened.ToString()}" + Environment.NewLine +
                        $"- SelfMuted: {gUser.IsSelfMuted.ToString()}" + Environment.NewLine +
                        $"- Suppressed: {gUser.IsSuppressed.ToString()}";
                });
            }
            b.AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Bot/Webhook";
                f.Value = $"- Bot: {user.IsBot.ToString()}" + Environment.NewLine +
                    $"- Webhook: {user.IsWebhook.ToString()}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Created at";
                f.Value = user.CreatedAt.UtcDateTime.ToString("dd/MM/yyyy HH:mm:ss");
            });
            if (gUser != null)
            {
                b.AddField(f =>
                {
                    f.IsInline = true;
                    f.Name = "Joined at";
                    f.Value = gUser.JoinedAt.Value.UtcDateTime.ToString("dd/MM/yyyy HH:mm:ss");
                });
            }
            await ReplyAsync("", embed: b);
        }
    }
}
