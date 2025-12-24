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

       await DownloadService.File($"{Definitions.CDN_URL}/EonMods.ucas", GamePath, "pakchunkEon-WindowsClient_p.ucas");
       await DownloadService.File($"{Definitions.CDN_URL}/EonMods.utoc", GamePath, "pakchunkEon-WindowsClient_p.utoc");
       await DownloadService.File($"{Definitions.CDN_URL}/S17_Univeral.pak", GamePath, "pakchunkEon-WindowsClient_p.pak");
       await DownloadService.File($"{Definitions.CDN_URL}/S17_Univeral.sig", GamePath, "pakchunkEon-WindowsClient_p.sig");
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
            await DownloadService.File($"{Definitions.CDN_URL}/LowMesh.ucas", GamePath, "pakchunkLowMesh-WindowsClient_p.ucas");
            await DownloadService.File($"{Definitions.CDN_URL}/LowMesh.utoc", GamePath, "pakchunkLowMesh-WindowsClient_p.utoc");
            await DownloadService.File($"{Definitions.CDN_URL}/S17_Univeral.pak", GamePath, "pakchunkLowMesh-WindowsClient_p.pak");
            await DownloadService.File($"{Definitions.CDN_URL}/S17_Univeral.sig", GamePath, "pakchunkLowMesh-WindowsClient_p.sig");
        }
    }
}