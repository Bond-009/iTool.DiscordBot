using iTool.DiscordBot.Prefrences;
using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OpenWeatherNet;

namespace iTool.DiscordBot.Modules
{
    public class Weather : ModuleBase
    {
        [Command("weather")]
        [Summary("Gets the weather info")]
        public async Task GetWeather(string input)
        {
            //TODO: Add weather
            OpenWeatherClient client = new OpenWeatherClient(Settings.ApiKeys.OpenWeatherMapKey);
            WeatherInfo weather = await client.GetCurrentAsync(input);
            weather.Temperature.ToCelsius(); //TODO: Add temperaturescale setting

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = weather.City.Name + " " + weather.City.Country,
                Color = new Color(3, 144, 255),
                ThumbnailUrl = weather.IconURL
            };
            b.AddField(delegate (EmbedFieldBuilder f)
            {
                f.Name = "Temperature";
                f.Value = $"Max: {weather.Temperature.Max} {weather.Temperature.Unit}" + Environment.NewLine +
                            $"Gem: {weather.Temperature.Value} {weather.Temperature.Unit}" + Environment.NewLine +
                            $"Min: {weather.Temperature.Min} {weather.Temperature.Unit}";
            });
            await ReplyAsync("", embed: b);
        }
    }
}