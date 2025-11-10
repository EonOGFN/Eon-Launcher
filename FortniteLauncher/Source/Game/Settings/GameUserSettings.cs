using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

class Client
{
    private static string GamePath => GlobalSettings.Options.FortnitePath;
    private static string LocalAppData => Environment.GetEnvironmentVariable("LocalAppData");
    private static string GameUserSettingsPath => Path.Combine(LocalAppData, "FortniteGame", "Saved", "Config", "WindowsClient", "GameUserSettings.ini");

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetFileAttributes(string FileName, FileAttributes Attributes);

    public static void ApplyWindowsClientFix()
    {
        MakeGameUserSettingsWritable();
        UnhideWin64Folder();
        UpdateLobbyBackground();
    }

    private static void UnhideWin64Folder()
    {
        string Win64Path = Path.Combine(GamePath, "FortniteGame", "Binaries", "Win64");
        if (!Directory.Exists(Win64Path)) return;

        var Attributes = File.GetAttributes(Win64Path);
        Attributes &= ~FileAttributes.Hidden;
        SetFileAttributes(Win64Path, Attributes);
    }

    private static void MakeGameUserSettingsWritable()
    {
        if (!File.Exists(GameUserSettingsPath)) return;

        var Attributes = File.GetAttributes(GameUserSettingsPath);
        if ((Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
        {
            Attributes &= ~FileAttributes.ReadOnly;
            File.SetAttributes(GameUserSettingsPath, Attributes);
        }
    }

    private static void UpdateLobbyBackground()
    {
        if (!File.Exists(GameUserSettingsPath)) return;

        string LobbyValue = GlobalSettings.Options.IsWCLobbyEnabled ? "worldcup" : "season17";
        UpdateConfigValue(GameUserSettingsPath, "LastFrontEndBackPlateStageUsed[0]", LobbyValue);
        UpdateConfigValue(GameUserSettingsPath, "LastFrontEndBackPlateStageUsed[1]", LobbyValue);
    }

    private static void UpdateConfigValue(string FilePath, string Key, string NewValue)
    {
        var Lines = File.ReadAllLines(FilePath);
        var UpdatedLines = new List<string>();

        foreach (var Line in Lines)
        {
            if (Line.Contains("="))
            {
                var Parts = Line.Split(new[] { '=' }, 2);
                if (Parts[0].Trim() == Key)
                {
                    UpdatedLines.Add($"{Key}={NewValue}");
                    continue;
                }
            }
            UpdatedLines.Add(Line);
        }

        File.WriteAllLines(FilePath, UpdatedLines);
    }
}