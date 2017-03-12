using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace iTool.DiscordBot.HOTS
{
    static class HOTSLogs
    {
        public static async Task<Player> GetPlayerSummary(int region, string battletag)
        {
            using (HttpClient httpclient = new HttpClient())
            {
                return await await Task.Factory.StartNew(async () =>
                    JsonConvert.DeserializeObject<Player>(
                        await httpclient.GetStringAsync(
                            $"https://api.hotslogs.com/Public/Players/{region}/{battletag}")));
            }
        }

        public static async Task<Player> GetPlayerSummary(int playerID)
        {
            using (HttpClient httpclient = new HttpClient())
            {
                return await await Task.Factory.StartNew(async () =>
                    JsonConvert.DeserializeObject<Player>(
                        await httpclient.GetStringAsync(
                            $"https://api.hotslogs.com/Public/Players/{playerID}")));
            }
        }
    }
}
