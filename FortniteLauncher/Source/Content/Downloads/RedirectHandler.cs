using System.IO;
using System.Threading.Tasks;
class RedirectHandler
{
    public static async Task DownloadFile()
    {
        if (!GlobalSettings.Options.RedirectProtected)
        {
            bool Question = await DialogService.YesOrNoDialog($"We've noticed that {ProjectDefinitions.Name} isn't excluded. This could prevent it from loading in the game. Would you like to add it to the exclusion list?", "Exclusion Warning");
            if (Question)
                await ExclusionManager.AddToExclusions(GlobalSettings.Options.FortnitePath);
        }

        string Path = $"{GlobalSettings.Options.FortnitePath}\\Engine\\Binaries\\ThirdParty\\NVIDIA\\NVaftermath\\Win64\\";
        if (!Directory.Exists(Path))
        {
            DialogService.ShowSimpleDialog($"It appears that your build is corrupted and the Redirect folder cannot be located. Please re-download Chapter {ProjectDefinitions.Chapter} Season {ProjectDefinitions.Season} ({ProjectDefinitions.Build}) to resolve this issue.", "Failed to Download Redirect");
            return;
        }

        DownloadService.File($"{Definitions.WebserverURL}/GFSDK_Aftermath_Lib.x64.dll", Path, "GFSDK_Aftermath_Lib.x64.dll");
    }
}

