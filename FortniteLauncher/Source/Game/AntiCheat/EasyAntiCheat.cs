using System;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Threading.Tasks;

public enum EACOperation { Initialize, Installation }

class EAC
{
    private static string GamePath = GlobalSettings.Options.FortnitePath;

    public static async Task Execute(EACOperation Operation)
    {
        if (!Definitions.bEnableEAC)
            return;

        switch (Operation)
        {
            case EACOperation.Initialize:
                await InitializeComponent();
                break;

            case EACOperation.Installation:
                await DeleteFiles();
                await DownloadFiles();
                await ExtractArchive();
                break;
        }
    }

    private static async Task DeleteFiles()
    {
        if (Directory.Exists(Path.Combine(GamePath, "EasyAntiCheat")))
            Directory.Delete(Path.Combine(GamePath, "EasyAntiCheat"), true);

        if (File.Exists(Path.Combine(GamePath, "Eon_EAC.exe")))
            File.Delete(Path.Combine(GamePath, "Eon_EAC.exe"));

        await Task.CompletedTask;
    }

    private static async Task DownloadFiles()
    {
        await DownloadService.File($"{Definitions.CDN_URL}/Eon_EAC.exe", GamePath, "Eon_EAC.exe");
        await DownloadService.File($"{Definitions.CDN_URL}/EasyAntiCheat.zip", GamePath, "EasyAntiCheat.zip");
    }

    private static async Task ExtractArchive()
    {
        ZipFile.ExtractToDirectory(Path.Combine(GamePath, "EasyAntiCheat.zip"), Path.Combine(GamePath, "EasyAntiCheat"));

        if (File.Exists(Path.Combine(GamePath, "EasyAntiCheat.zip")))
            File.Delete(Path.Combine(GamePath, "EasyAntiCheat.zip"));

        await Task.CompletedTask;
    }

    private static async Task InitializeComponent()
    {
        using var AntiCheat = new Process
        {
            StartInfo = new ProcessStartInfo(Path.Combine(GamePath, "EasyAntiCheat", "EasyAntiCheat_EOS_Setup.exe"))
            {
                Arguments = "install \"c557c546364948a39015f9b7106e36c0\"",
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            }
        };

        AntiCheat.Start();
        AntiCheat.WaitForExit();

        await Task.CompletedTask;
    }
}