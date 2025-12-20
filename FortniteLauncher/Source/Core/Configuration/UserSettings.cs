using System;
using System.IO;
using Newtonsoft.Json;

class UserSettings
{
    private static readonly string RootDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Project Eon");

    private static readonly string SaveFile = Path.Combine(RootDirectory, "Config.json");

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
            Username = string.Empty,
            Email = string.Empty,
            Password = string.Empty,
            FortnitePath = string.Empty,
            IsSoundEnabled = false,
            IsBubbleBuildsEnabled = false,
            RedirectProtected = false,
            SkinUrl = string.Empty,
        };
    }
}