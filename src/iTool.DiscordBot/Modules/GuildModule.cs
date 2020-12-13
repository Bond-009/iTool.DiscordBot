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
        private readonly Settings _settings;

        public GuildModule(Settings settings) => _settings = settings;

        [Command("prefix")]
        [Summary("Returns the current prefix")]
        public async Task Prefix()
        {
            GuildSettings guildSettings = await _db.GetSettingsAsync(Context.Guild.Id).ConfigureAwait(false);

            await ReplyAsync(string.Empty, embed: new EmbedBuilder()
            {
                Title = $"Current prefix",
                Color = _settings.GetColor(),
                Description = string.IsNullOrEmpty(guildSettings.Prefix) ? _settings.Prefix : guildSettings.Prefix,
            }.Build()).ConfigureAwait(false);
        }

        [Command("prefix")]
        [Alias("setprefix", "prefix set")]
        [Summary("Sets the current prefix")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Prefix(string prefix)
        {
            GuildSettings guildSettings = await _db.GetSettingsAsync(Context.Guild.Id).ConfigureAwait(false);
            guildSettings.Prefix = prefix;
            await _db.UpdateSettings(guildSettings).ConfigureAwait(false);

            await ReplyAsync(string.Empty, embed: new EmbedBuilder()
            {
                Title = $"Prefix",
                Color = _settings.GetColor(),
                Description = $"Changed to prefix for this server to {prefix}.",
            }.Build()).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
