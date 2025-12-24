using System;
using System.Threading.Tasks;

class Fortnite
{
    public static async Task Launch(string GamePath)
    {
        try
        {
            await RequiredFilesDownloader.Download();
            if (Anticheat.CheckForCorruption(GamePath) != Anticheat.EPlayStatus.Playable)
                return;

            await EAC.Execute(EACOperation.Initialize);
            await FNProc.Launch($"{GamePath}\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe");
            await FNProc.Launch($"{GamePath}\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_BE.exe");
            await FNProc.Launch($"{GamePath}\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_EAC.exe");
            await FNProc.Launch($"{GamePath}\\Eon_EAC.exe", false, $"-AUTH_LOGIN={GlobalSettings.Options.Email} -AUTH_PASSWORD={GlobalSettings.Options.Password}");

            LaunchStatusService.OnGameOpened();
        }
        catch (Exception Error)
        {
            DialogService.ShowSimpleDialog($"{Error.Message}", "Whoops! Something went wrong.");
        }
    }
}