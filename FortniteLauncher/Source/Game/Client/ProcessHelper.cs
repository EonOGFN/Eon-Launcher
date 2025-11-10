using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
class FNProc
{
    public static async Task<Process> Launch(string GamePath, bool FreezeProc = true, string Arg = "")
    {
        Process Process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = (GamePath.Contains($"{ProjectDefinitions.Name}_EAC.exe") && !Definitions.bEnableEAC) ? $"{GlobalSettings.Options.FortnitePath}\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe" : GamePath,
                Arguments = $"{Arg} -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -nobe -fromfl=eac -skippatchcheck -fltoken=3db3ba5dcbd2e16703f3978d -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -AUTH_TYPE=epic",
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = Path.GetDirectoryName((GamePath.Contains($"{ProjectDefinitions.Name}_EAC.exe") && !Definitions.bEnableEAC) ? $"{GlobalSettings.Options.FortnitePath}\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe" : GamePath)
            }
        };

        Process.Start();

        if (FreezeProc)
        {
            foreach (ProcessThread ProcessThread in Process.Threads)
            {
                var ThreadHandle = OpenThread(THREAD_SUSPEND_RESUME, false, ProcessThread.Id);
                if (ThreadHandle != IntPtr.Zero)
                {
                    SuspendThread(ThreadHandle);
                    CloseHandle(ThreadHandle);
                }
            }
        }

        return Process;
    }

    private const int THREAD_SUSPEND_RESUME = 0x0002;

    [DllImport("kernel32.dll")]
    public static extern int SuspendThread(IntPtr hThread);

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenThread(int dwDesiredAccess, bool bInheritHandle, int dwThreadId);

    [DllImport("kernel32.dll")]
    public static extern bool CloseHandle(IntPtr hObject);
}