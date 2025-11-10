using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

class EAC
{
    private static string GamePath = GlobalSettings.Options.FortnitePath;

    public static async Task DeleteFiles()
    {
        if (Directory.Exists($"{GamePath}\\EasyAntiCheat"))
            Directory.Delete($"{GamePath}\\EasyAntiCheat", true);

        if (File.Exists($"{GamePath}\\Eon_EAC.exe"))
            File.Delete($"{GamePath}\\Eon_EAC.exe");
    }

    public static async Task ExtractEAC()
    {
        string EACZipPath = $"{GamePath}\\EasyAntiCheat.zip";
        await DownloadAndExtractArchive(EACZipPath, $"{GamePath}\\EasyAntiCheat");

        if (File.Exists(EACZipPath))
            File.Delete(EACZipPath);
    }

    public static async Task DownloadAndExtractArchive(string FilePath, string ExtractPath)
    {
        await DownloadService.File($"{Definitions.WebserverURL}/Eon_EAC.exe", GamePath, "Eon_EAC.exe");
        await DownloadService.File($"{Definitions.WebserverURL}/EasyAntiCheat.zip", GamePath, Path.GetFileName(FilePath));

        ZipFile.ExtractToDirectory(FilePath, ExtractPath);
    }

    public static async Task InitializeComponent()
    {
        if (!Definitions.bEnableEAC)
            return;

        Process AntiCheat = new Process()
        {
            StartInfo = new ProcessStartInfo($"{GamePath}\\EasyAntiCheat\\EasyAntiCheat_EOS_Setup.exe")
            {
                Arguments = "install \"c557c546364948a39015f9b7106e36c0\"",
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            }
        };

        AntiCheat.Start();
        AntiCheat.WaitForExit();
    }
}