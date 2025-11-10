using Newtonsoft.Json;
using System;
using System.IO;

class UserSettings
{
    private static readonly string RootDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Eon Launcher");

    private static readonly string SaveFile = Path.Combine(RootDirectory, "settings.json");

    public static void SaveSettings()
    {
        var Json = JsonConvert.SerializeObject(GlobalSettings.Options, Formatting.Indented);
        Directory.CreateDirectory(RootDirectory);
        File.WriteAllText(SaveFile, Json);
    }

    public static void LoadSettings()
    {
        try
        {
            if (File.Exists(SaveFile))
            {
                var Json = File.ReadAllText(SaveFile);
                GlobalSettings.Options = IsValidJson(Json) ? JsonConvert.DeserializeObject<AppConfig>(Json) : GetDefaultConfig();
            }
            else
            {
                GlobalSettings.Options = GetDefaultConfig();
                SaveSettings();
            }
        }
        catch (Exception)
        {
            GlobalSettings.Options = GetDefaultConfig();
            SaveSettings();
        }
    }

    private static bool IsValidJson(string Json)
    {
        if (string.IsNullOrWhiteSpace(Json))
            return false;

        Json = Json.Trim();
        return (Json.StartsWith("{") && Json.EndsWith("}")) || (Json.StartsWith("[") && Json.EndsWith("]"));
    }

    private static AppConfig GetDefaultConfig()
    {
        return new AppConfig
        {
            IsSoundEnabled = true,
            IsWCLobbyEnabled = false,
            FortnitePath = string.Empty,
            Email = string.Empty,
            Password = string.Empty
        };
    }
}