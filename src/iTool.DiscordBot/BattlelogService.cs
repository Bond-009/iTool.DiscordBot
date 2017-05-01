using LiteDB;
using System;
using System.IO;
using System.Linq;

namespace iTool.DiscordBot
{
    public class BattlelogService : IDisposable
    {
        LiteDatabase db;
        public BattlelogService()
            => db = new LiteDatabase(Common.DataDir + Path.DirectorySeparatorChar + "bfpersonaids.db");

        public long? GetPersonaID(string name)
        {
            LiteCollection<BfPlayer> col = db.GetCollection<BfPlayer>("bfplayers");

            return col.Find(x => x.Name == name).FirstOrDefault()?.PersonaID;
        }

        public void SavePersonaID(string name, long id)
            => SavePersonaID(new BfPlayer()
            {
                Name = name,
                PersonaID = id
            });

        public void SavePersonaID(BfPlayer name)
        {
                LiteCollection<BfPlayer> col = db.GetCollection<BfPlayer>("bfplayers");

                if (!col.EnsureIndex(x => x.Name, true)) return;

                col.Insert(name);
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
