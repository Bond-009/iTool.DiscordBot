using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace iTool.DiscordBot
{
    public sealed class GuildSettingsDatabase : DbContext
    {
        public DbSet<GuildSettings> GuildConfigs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!Directory.Exists(Common.DataDir))
            {
                Directory.CreateDirectory(Common.DataDir);
            }

            optionsBuilder.UseSqlite($"Filename={Path.Combine(Common.DataDir, "guildsettings.sqlite.db")}");
        }

        public async Task<GuildSettings> GetSettingsAsync(ulong guildID)
        {
            GuildSettings settings = await GuildConfigs.FirstOrDefaultAsync(x => x.GuildID == guildID).ConfigureAwait(false);

            if (settings != null)
            {
                return settings;
            }

            GuildConfigs.Add(settings = new GuildSettings()
            {
                GuildID = guildID
            });
            await SaveChangesAsync().ConfigureAwait(false);
            return settings;
        }

        public async Task UpdateSettings(GuildSettings guildSettings)
        {
            GuildConfigs.Update(guildSettings);
            await SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
