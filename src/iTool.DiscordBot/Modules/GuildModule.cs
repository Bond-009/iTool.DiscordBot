using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace iTool.DiscordBot.Modules
{
    [RequireGuildSpecificSettings]
    public class GuildModule : ModuleBase, IDisposable
    {
        private readonly GuildSettingsDatabase _db = new GuildSettingsDatabase();
        private Settings _settings;

        public GuildModule(Settings settings) => _settings = settings;

        [Command("prefix")]
        [Summary("Returns the current prefix")]
        public async Task Prefix()
        {
            GuildSettings guildSettings = await _db.GetSettingsAsync(Context.Guild.Id);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Current prefix",
                Color = _settings.GetColor(),
                Description = guildSettings.Prefix.IsNullOrEmpty() ? _settings.Prefix : guildSettings.Prefix,
            });
        }

        [Command("prefix")]
        [Alias("setprefix", "prefix set")]
        [Summary("Sets the current prefix")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Prefix(string prefix)
        {
            GuildSettings guildSettings = await _db.GetSettingsAsync(Context.Guild.Id);
            guildSettings.Prefix = prefix;
            await _db.UpdateSettings(guildSettings);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = $"Prefix",
                Color = _settings.GetColor(),
                Description = $"Changed to prefix for this server to {prefix}.",
            });
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
