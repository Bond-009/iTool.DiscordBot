using System.Collections.Generic;

namespace iTool.DiscordBot.HOTS
{
    public class Player
    {
        public int PlayerID { get; set; }
        public string Name { get; set; }
        public List<Ranking> LeaderboardRankings { get; set; }
    }
}
