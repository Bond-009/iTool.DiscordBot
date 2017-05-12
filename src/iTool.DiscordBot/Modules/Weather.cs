using Discord;
using Discord.Commands;
using OpenWeather;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Weather : ModuleBase
    {
        static OpenWeatherClient client;
        Settings settings;

        public Weather(Settings settings)
        {
            if (string.IsNullOrEmpty(settings.OpenWeatherMapKey))
            {
                throw new Exception("No OpenWeatherMapKey found.");
            }

            if (client == null)
            { client = new OpenWeatherClient(settings.OpenWeatherMapKey, settings.Units); }

            this.settings = settings;
        }

        [Command("weather")]
        [Summary("Returns info about the weather")]
        public async Task GetWeather(string city, string countryCode = null)
        {
            WeatherData weather = await client.GetWeatherAsync(city, countryCode);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = weather.Name + " " + weather.Sys.Country,
                Color = settings.GetColor(),
                ThumbnailUrl = client.GetIconURL(weather.Weather.FirstOrDefault()?.Icon),
                Footer = new EmbedFooterBuilder()
                    {
                        Text = "Powered by openweathermap.org",
                    }
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Temperature";
                f.Value = $"- Max: {weather.Main.MaximumTemperature} {GetTemperatureUnit(settings.Units)}" + Environment.NewLine +
                            $"- Gem: {weather.Main.Temperature} {GetTemperatureUnit(settings.Units)}" + Environment.NewLine +
                            $"- Min: {weather.Main.MinimumTemperature} {GetTemperatureUnit(settings.Units)}";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Humidity";
                f.Value = weather.Main.Humidity + "%";
            })
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Wind";
                f.Value = weather.Wind.Speed + " " + GetSpeedUnit(settings.Units);
            }));
        }

        static string GetTemperatureUnit(Unit units)
        {
            switch(units)
            {
                case Unit.Imperial:
                    return "°F";
                case Unit.Metric:
                    return "°C";
                case Unit.Standard:
                default:
                    return "kelvin";
            }
        }

        static string GetSpeedUnit(Unit units)
        {
            switch(units)
            {
                case Unit.Imperial:
                    return "mi/h";
                case Unit.Metric:
                case Unit.Standard:
                default:
                    return "m/s";
            }
        }
    }
}
