using System.Diagnostics;
using System.Threading.Tasks;
class ExclusionManager
{
    public static async Task AddToExclusions(string Path)
    {
        try
        {
            GlobalSettings.Options.RedirectProtected = true;
            UserSettings.SaveSettings();

            var PSI = new ProcessStartInfo("powershell.exe", $"-Command Add-MpPreference -ExclusionPath \"{Path}\"")
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runas" 
            };

            using var StartProcess = Process.Start(PSI);
            StartProcess.WaitForExit();
        }
        catch  
        { 
            GlobalSettings.Options.RedirectProtected = false; 
            UserSettings.SaveSettings(); 
        }
    }
}