using System.IO;
using System.Threading.Tasks;

class PakChunk
{
    private static string GamePath = $"{GlobalSettings.Options.FortnitePath}\\FortniteGame\\Content\\Paks\\";
    public static async Task EonPak()
    {
        if (!Directory.Exists(GamePath))
        {
            DialogService.ShowSimpleDialog(string.Empty, "Corrupted Data Detected");
            return;
        }

        foreach (var Mods in Directory.GetFiles(GamePath, "*Eon*"))
            File.Delete(Mods);

        // Parallel downloads for better performance
        var downloadTasks = new Task[]
        {
            DownloadService.DownloadFile($"{Definitions.CDN_URL}/EonMods.ucas", GamePath, "pakchunkEon-WindowsClient_p.ucas"),
            DownloadService.DownloadFile($"{Definitions.CDN_URL}/EonMods.utoc", GamePath, "pakchunkEon-WindowsClient_p.utoc"),
            DownloadService.DownloadFile($"{Definitions.CDN_URL}/S17_Univeral.pak", GamePath, "pakchunkEon-WindowsClient_p.pak"),
            DownloadService.DownloadFile($"{Definitions.CDN_URL}/S17_Univeral.sig", GamePath, "pakchunkEon-WindowsClient_p.sig")
        };

        await Task.WhenAll(downloadTasks);
    }

    public static async Task BubbleBuilds()
    {
        if (!Directory.Exists(GamePath))
        {
            DialogService.ShowSimpleDialog(string.Empty, "Corrupted Data Detected");
            return;
        }

        foreach (var Mods in Directory.GetFiles(GamePath, "*LowMesh*"))
            File.Delete(Mods);

        if (GlobalSettings.Options.IsBubbleBuildsEnabled)
        {
            // Parallel downloads for better performance
            var downloadTasks = new Task[]
            {
                DownloadService.DownloadFile($"{Definitions.CDN_URL}/LowMesh.ucas", GamePath, "pakchunkLowMesh-WindowsClient_p.ucas"),
                DownloadService.DownloadFile($"{Definitions.CDN_URL}/LowMesh.utoc", GamePath, "pakchunkLowMesh-WindowsClient_p.utoc"),
                DownloadService.DownloadFile($"{Definitions.CDN_URL}/S17_Univeral.pak", GamePath, "pakchunkLowMesh-WindowsClient_p.pak"),
                DownloadService.DownloadFile($"{Definitions.CDN_URL}/S17_Univeral.sig", GamePath, "pakchunkLowMesh-WindowsClient_p.sig")
            };

            await Task.WhenAll(downloadTasks);
        }
    }
}