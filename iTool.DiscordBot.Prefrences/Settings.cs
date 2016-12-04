using System;
using System.IO;
using System.Xml;

namespace iTool.DiscordBot.Prefrences
{
	public static class Settings
	{
		#region Settings

		public static class General
		{
			public static string Game;
			public static bool AntiSwear;
		}

		public static class ApiKeys
		{
			public static string SteamKey;
			public static string OpenWeatherMapKey;
			public static string DiscordToken;
		}

			public static class Static
		{	
			public static readonly string SettingsDir = AppContext.BaseDirectory + Path.DirectorySeparatorChar + "settings";
			public static readonly string SettingsFile = SettingsDir + Path.DirectorySeparatorChar + "settings.xml";
		}

		#endregion

		public static void Load()
		{
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(File.ReadAllText(Static.SettingsFile));
				//General
				General.Game = xmlDoc["iTool"]["General"]["Game"].InnerText;
				General.AntiSwear = Convert.ToBoolean(xmlDoc["iTool"]["General"]["AntiSwear"].InnerText);
				//Api keys
				ApiKeys.SteamKey = xmlDoc["iTool"]["ApiKeys"]["SteamKey"].InnerText;
				ApiKeys.OpenWeatherMapKey = xmlDoc["iTool"]["ApiKeys"]["OpenWeatherMapKey"].InnerText;
				ApiKeys.DiscordToken = xmlDoc["iTool"]["ApiKeys"]["DiscordToken"].InnerText;
			}
			catch (FileNotFoundException)
			{
				Reset();
			}
			catch (DirectoryNotFoundException)
			{
				Reset();
			}
		}

		public static void Save()
		{
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(File.ReadAllText(Static.SettingsFile));
				//General
				xmlDoc["iTool"]["General"]["Game"].InnerText = General.Game;
				xmlDoc["iTool"]["General"]["AntiSwear"].InnerText = General.AntiSwear.ToString();

				xmlDoc.Save(File.Open(Static.SettingsFile, FileMode.OpenOrCreate));
			}
			catch { Console.WriteLine("Can't save settings."); }
		}

		public static void Reset()
		{
			if (File.Exists(Static.SettingsFile))
			{
				File.Delete(Static.SettingsFile);
				SettingsFolders.Create();
				Load();
				Console.WriteLine("Settings reset.");
			}
			else
			{
				SettingsFolders.Create();
				Load();
			}
		}
	}
}
