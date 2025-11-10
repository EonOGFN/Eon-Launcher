using System.IO;

class PathHelper
{
    public static bool IsPathValid(string Path)
    {
        if (string.IsNullOrEmpty(Path) || !Directory.Exists(Path)) return false;

        string EnginePath = System.IO.Path.Combine(Path, "Engine");
        string FortniteGamePath = System.IO.Path.Combine(Path, "FortniteGame");

        return Directory.Exists(EnginePath) && Directory.Exists(FortniteGamePath);
    }

    public static string FindValidInstallationPath(string SelectedPath)
    {
        if (string.IsNullOrEmpty(SelectedPath) || !Directory.Exists(SelectedPath))
            return null;

        DirectoryInfo Current = new DirectoryInfo(SelectedPath);

        while (Current != null)
        {
            if (IsPathValid(Current.FullName))
                return Current.FullName;

            Current = Current.Parent;
        }

        return null;
    }
}