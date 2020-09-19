using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace iTool.DiscordBot
{
    public class BfPlayerDatabase : DbContext
    {
        public DbSet<BfPlayer> BfPlayers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!Directory.Exists(Common.DataDir)) Directory.CreateDirectory(Common.DataDir);

            optionsBuilder.UseSqlite($"Filename={Path.Combine(Common.DataDir, "bfpersonaids.sqlite.db")}");
        }

        public async Task<long?> GetPersonaIDAsync(string name)
            => (await BfPlayers.FirstOrDefaultAsync(x => x.Name == name).ConfigureAwait(false))?.PersonaID;

        public Task SavePersonaIDAsync(string name, long id)
            => SavePersonaIDAsync(new BfPlayer()
            {
                Name = name,
                PersonaID = id
            });

        public async Task SavePersonaIDAsync(BfPlayer player)
        {
            if (await BfPlayers.AnyAsync(x => x.Name == player.Name).ConfigureAwait(false))
            {
                throw new ArgumentException($"A player with name {player.Name} already exists.");
            }

            BfPlayers.Add(player);

            await SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
