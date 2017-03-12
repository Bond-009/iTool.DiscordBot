namespace iTool.DiscordBot.HOTS
{
    public class Ranking
    {
        public string GameMode { get; set; }
        public int? LeagueID { get; set; }
        public int? LeagueRank { get; set; }
        public int CurrentMMR { get; set; }
    }
}
