using Discord;
using Discord.Commands;
using OpenWeather;
using System;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Weather : ModuleBase
    {
        [Command("weather")]
        [Summary("Returns info about the weather")]
        public async Task GetWeather(string input)
        {
            if (string.IsNullOrEmpty(Program.Settings.OpenWeatherMapKey))
            {
                await Program.Log(new LogMessage(LogSeverity.Warning, "", "No OpenWeatherMapKey found."));
                return;
            }

            OpenWeatherClient client = new OpenWeatherClient(Program.Settings.OpenWeatherMapKey);
            WeatherInfo weather = await client.GetCurrentAsync(input);
            weather.Temperature = weather.Temperature.ToCelsius(); //TODO: Add temperaturescale setting

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = weather.City.Name + " " + weather.City.Country,
                Color = new Color(3, 144, 255),
                ThumbnailUrl = weather.IconURL
            };
            b.AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Temperature";
                f.Value = $"Max: {weather.Temperature.Max} {weather.Temperature.Unit}" + Environment.NewLine +
                            $"Gem: {weather.Temperature.Value} {weather.Temperature.Unit}" + Environment.NewLine +
                            $"Min: {weather.Temperature.Min} {weather.Temperature.Unit}";
            });

            if (weather.Precipitation.Value != 0)
            {
                b.AddField(f =>
                {
                    f.IsInline = true;
                    f.Name = "Precipation";
                    f.Value = weather.Precipitation.Value + weather.Precipitation.Unit;
                });
            }

            b.AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Humidity";
                f.Value = weather.Humidity.Value + weather.Humidity.Unit;
            });
            b.AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Wind";
                f.Value = weather.Wind.Speed.Name + Environment.NewLine +
                            weather.Wind.Speed.Value + "m/s";
            });
            await ReplyAsync("", embed: b);
        }
    }
}
