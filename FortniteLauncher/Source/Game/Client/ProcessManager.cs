using System.Diagnostics;
using System.Threading.Tasks;

class Processes
{
    public static void Kill(string ProcessName)
    {
        Process[] Processes = Process.GetProcessesByName(ProcessName);
        foreach (Process Process in Processes)
        {
            Process.Kill();
        }
    }

    public static async Task ForceCloseFortnite(bool OnErrorCode = false)
    {
        if (OnErrorCode)
           LaunchStatusService.OnGameClosed();

        Kill("FModel");

        Kill("FortniteLauncher");
        Kill("FortniteClient-Win64-Shipping");
        Kill("FortniteClient-Win64-Shipping_EAC");
        Kill("FortniteClient-Win64-Shipping_BE");

        Kill("Eon_EAC");
        Kill("Easy Anti-Cheat Bootstrapper");

        Kill("EpicGamesLauncher");
        Kill("CrashReportClient");
    }
}
