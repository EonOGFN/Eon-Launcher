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

       await DownloadService.File($"{Definitions.WebserverURL}/S17_Univeral.pak", GamePath, "z_pakchunkEon-WindowsClient_P.pak");
       await DownloadService.File($"{Definitions.WebserverURL}/S17_Univeral.sig", GamePath, "z_pakchunkEon-WindowsClient_P.sig");
       await DownloadService.File($"{Definitions.WebserverURL}/z_pakchunkEon-WindowsClient_P.ucas", GamePath, "z_pakchunkEon-WindowsClient_P.ucas");
       await DownloadService.File($"{Definitions.WebserverURL}/z_pakchunkEon-WindowsClient_P.utoc", GamePath, "z_pakchunkEon-WindowsClient_P.utoc");
    }

    public static async Task BubbleBuilds()
    {
        if (!Directory.Exists(GamePath))
        {
            DialogService.ShowSimpleDialog(string.Empty, "Corrupted Data Detected");
            return;
        }

        foreach (var Mod in new[] { "z_pakchunkLowMesh-WindowsClient_P.pak", "z_pakchunkLowMesh-WindowsClient_P.sig", "z_pakchunkLowMesh-WindowsClient_P.ucas", "z_pakchunkLowMesh-WindowsClient_P.utoc" })
            File.Delete(Path.Combine(GamePath, Mod));

        if (GlobalSettings.Options.IsBubbleBuildsEnabled)
        {
            await DownloadService.File($"{Definitions.WebserverURL}/S17_Univeral.pak", GamePath, "z_pakchunkLowMesh-WindowsClient_P.pak");
            await DownloadService.File($"{Definitions.WebserverURL}/S17_Univeral.sig", GamePath, "z_pakchunkLowMesh-WindowsClient_P.sig");
            await DownloadService.File($"{Definitions.WebserverURL}/z_pakchunkLowMesh-WindowsClient_P.ucas", GamePath, "z_pakchunkLowMesh-WindowsClient_P.ucas");
            await DownloadService.File($"{Definitions.WebserverURL}/z_pakchunkLowMesh-WindowsClient_P.utoc", GamePath, "z_pakchunkLowMesh-WindowsClient_P.utoc");
        }
    }
}