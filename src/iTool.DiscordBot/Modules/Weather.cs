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
                await Program.Log(new LogMessage(LogSeverity.Warning, nameof(Weather), "No OpenWeatherMapKey found."));
                return;
            }

            WeatherInfo weather = await Program.OpenWeatherClient.GetCurrentAsync(input);
            switch (Program.Settings.TemperatureScale)
            {
                case TemperatureScale.Kelvin:
                    weather.Temperature = weather.Temperature.ToKelvin();
                    break;
                case TemperatureScale.Fahrenheit:
                    weather.Temperature = weather.Temperature.ToFahrenheit();
                    break;
                case TemperatureScale.Celsius:
                default:
                    weather.Temperature = weather.Temperature.ToCelsius();
                    break;
            }

            EmbedBuilder b = new EmbedBuilder()
            {
                Title = weather.City.Name + " " + weather.City.Country,
                Color = new Color((uint)Colors.DodgerBlue),
                ThumbnailUrl = Program.OpenWeatherClient.GetIconURL(weather.Weather.Icon),
                Footer = new EmbedFooterBuilder()
                    {
                        Text = "Powered by openweathermap.org",
                    }
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Temperature";
                f.Value = $"- Max: {weather.Temperature.Max} {weather.Temperature.Unit}" + Environment.NewLine +
                            $"- Gem: {weather.Temperature.Value} {weather.Temperature.Unit}" + Environment.NewLine +
                            $"- Min: {weather.Temperature.Min} {weather.Temperature.Unit}";
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
