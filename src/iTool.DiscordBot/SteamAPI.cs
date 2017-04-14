using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    public class SteamAPI : IDisposable
    {
        HttpClient httpClient = new HttpClient();
        string key;

        public SteamAPI(string apiKey)
        {
            this.key = apiKey;
            httpClient.BaseAddress = new Uri("https://api.steampowered.com");
        }

        public async Task<PlayerList<PlayerBan>> GetPlayerBans(ulong[] steamIDs)
        {
            using (Stream stream = await httpClient.GetStreamAsync(
                $"/ISteamUser/GetPlayerBans/v1/?key={key}&steamids={string.Join(",", steamIDs)}&format=xml"))
            {
                return (PlayerList<PlayerBan>)new XmlSerializer(typeof(PlayerList<PlayerBan>)).Deserialize(stream);
            }
        }

        public async Task<PlayerList<PlayerSummary>> GetPlayerSummaries(ulong[] steamIDs)
        {
            using (Stream stream = await httpClient.GetStreamAsync(
                $"/ISteamUser/GetPlayerSummaries/v0002/?key={key}&steamids={string.Join(",", steamIDs)}&format=xml"))
            {
                return (PlayerList<PlayerSummary>)new XmlSerializer(typeof(PlayerList<PlayerSummary>)).Deserialize(stream);
            }
        }

        public async Task<UserStatsForGame> GetUserStatsForGame(int gameID, ulong steamID)
        {
            using (Stream stream = await httpClient.GetStreamAsync(
                $"/ISteamUserStats/GetUserStatsForGame/v0002/?key={key}&appid={gameID}&steamid={steamID}&format=xml"))
            {
                return (UserStatsForGame)new XmlSerializer(typeof(UserStatsForGame)).Deserialize(stream);
            }
        }

        public async Task<ulong> ResolveVanityURL(string playername)
        {

            using (Stream stream = await httpClient.GetStreamAsync(
                $"/ISteamUser/ResolveVanityURL/v0001/?key={key}&vanityurl={playername}&format=xml"))
            {
                VanityURL vanityurl = (VanityURL)new XmlSerializer(typeof(VanityURL)).Deserialize(stream);

                switch (vanityurl.Succes)
                {
                    case 1:
                        return vanityurl.SteamID64.Value;
                    case 42:
                        throw new Exception("No player found.");
                    default:
                        throw new Exception();
                }
            }
        }

        public void Dispose()
        {
            key = null;
            httpClient.Dispose();
        }
    }
}
