using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<PlayerBan>> GetPlayerBans(ulong[] steamIDs)
        {
            return await await Task.Factory.StartNew(async () =>
                JsonConvert.DeserializeObject<PlayerList<PlayerBan>>(
                    await httpClient.GetStringAsync(
                        $"/ISteamUser/GetPlayerBans/v1/?key={key}&steamids={string.Join(",", steamIDs)}"
            )).Players);
        }

        public async Task<IEnumerable<PlayerSummary>> GetPlayerSummaries(ulong[] steamIDs)
        {
            return await await Task.Factory.StartNew(async () =>
                JsonConvert.DeserializeObject<SteamResponse<PlayerList<PlayerSummary>>>(
                    await httpClient.GetStringAsync(
                        $"/ISteamUser/GetPlayerSummaries/v0002/?key={key}&steamids={string.Join(",", steamIDs)}"
            )).Data.Players);
        }

        public async Task<UserStatsForGame> GetUserStatsForGame(int gameID, ulong steamID)
        {
            return await await Task.Factory.StartNew(async () =>
                JsonConvert.DeserializeObject<UserStatsForGameResponse>(
                    await httpClient.GetStringAsync(
                        $"/ISteamUserStats/GetUserStatsForGame/v0002/?key={key}&appid={gameID}&steamid={steamID}"
            )).Data);
        }

        public async Task<ulong> ResolveVanityURL(string playername)
        {
            VanityURL vanityurl = await await Task.Factory.StartNew(async () =>
                JsonConvert.DeserializeObject<VanityURlResponse>(
                    await httpClient.GetStringAsync(
                        $"/ISteamUser/ResolveVanityURL/v0001/?key={key}&vanityurl={playername}"
            )).Data);

            switch (vanityurl.Success)
            {
                case 1:
                    return vanityurl.SteamID64.Value;
                case 42:
                    throw new Exception("No player found.");
                default:
                    throw new Exception();
            }
        }

        public void Dispose()
        {
            key = null;
            httpClient.Dispose();
        }
    }
}
