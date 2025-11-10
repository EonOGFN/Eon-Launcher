using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using Windows.Storage.Pickers;
using System.Linq;
using System.Diagnostics;

namespace FortniteLauncher.Pages
{
    public sealed partial class DownloadsPage : Page
    {
        private string CurrentPath;
        private string BuildPath;

        private string DownloadTitle = $"Downloading {ProjectDefinitions.Build} Build";

        private string Verify_Header = $"Scan {ProjectDefinitions.Name}";
        private string Season = $"{ProjectDefinitions.Name} Build ({ProjectDefinitions.Build}-CL-{ProjectDefinitions.ContentLevel})";

        private string Install = $"Install {ProjectDefinitions.Name}";
        private string Install_Body = $"Download the {ProjectDefinitions.Build} Fortnite Build, essential for playing {ProjectDefinitions.Name}.";

        private string Uninstall_Header = $"Uninstall {ProjectDefinitions.Name}";
        private string Uninstall_Body = $"Delete Chapter {ProjectDefinitions.Chapter} Season {ProjectDefinitions.Season} from your computer. This will not uninstall the {ProjectDefinitions.Name} Launcher.";

        public DownloadsPage()
        {
            this.InitializeComponent();
            InitializeBuildPath();
        }

        private void InitializeBuildPath()
        {
            if (GlobalSettings.Options.FortnitePath == null || !PathHelper.IsPathValid(GlobalSettings.Options.FortnitePath))
            {
                CurrentPath = "Game Path";
                BuildPath = "Path must contain \"FortniteGame\" and \"Engine\" folders!";
                return;
            }

            CurrentPath = GlobalSettings.Options.FortnitePath;
            BuildPath = $"This is the current build path for Fortnite Chapter {ProjectDefinitions.Chapter} Season {ProjectDefinitions.Season}.";
        }

        private async void DeleteBuild(object Sender, RoutedEventArgs Event)
        {
            bool Result = await DialogService.YesOrNoDialog($"Are you sure you want to remove Fortnite Version {ProjectDefinitions.Build} from your computer? This action cannot be undone.",$"Deleting {ProjectDefinitions.Name}");

            if (Result)
            {
                try
                {
                    if (Directory.Exists(GlobalSettings.Options.FortnitePath))
                    {
                        DirectoryInfo Directory = new DirectoryInfo(GlobalSettings.Options.FortnitePath);

                        foreach (FileInfo File in Directory.GetFiles())
                        {
                            File.Attributes = System.IO.FileAttributes.Normal;
                            File.Delete();
                        }

                        foreach (DirectoryInfo Dir in Directory.GetDirectories())
                        {
                            Dir.Attributes = System.IO.FileAttributes.Normal;
                            Dir.Delete(true);
                        }

                        Directory.Delete(true);

                        DialogService.ShowSimpleDialog($"{ProjectDefinitions.Name} has been successfully removed from your computer.", "Removal Confirmation");
                    }
                    else
                    {
                        DialogService.ShowSimpleDialog($"Could not find the Fortnite Version at the specified location.", "Not Found");
                    }
                }
                catch (Exception Ex)
                {
                    DialogService.ShowSimpleDialog($"{Ex.Message}", "An error occurred please report this to a moderator.");
                }
            }
            else
            {
                DialogService.ShowSimpleDialog($"Your request to remove Fortnite Version {ProjectDefinitions.Build} has been canceled. No changes were made.", "Cancellation Confirmed");
            }
        }

        private async void ChangeInstallPath(object Sender, RoutedEventArgs Event)
        {
            try
            {
                var OpenPicker = new FolderPicker();
                var HWnd = WinRT.Interop.WindowNative.GetWindowHandle(GlobalSettings.Windows);
                WinRT.Interop.InitializeWithWindow.Initialize(OpenPicker, HWnd);
                OpenPicker.ViewMode = PickerViewMode.Thumbnail;
                OpenPicker.FileTypeFilter.Add("*");

                var Folder = await OpenPicker.PickSingleFolderAsync();
                if (Folder == null)
                {
                    DialogService.ShowSimpleDialog("No folder was selected. Please select a valid installation folder.", "No Folder Selected");
                    return;
                }

                string FolderPath = Folder.Path;
                string[] CompressedExtensions = { ".rar", ".zip" };

                if (CompressedExtensions.Any(Ext => FolderPath.EndsWith(Ext, StringComparison.OrdinalIgnoreCase)))
                {
                    DialogService.ShowSimpleDialog("The selected file appears to be compressed. Please extract it using a third party extraction tool. You can find extraction guides on YouTube.", "Compressed File Error");
                    return;
                }

                if (!PathHelper.IsPathValid(FolderPath))
                {
                    string ValidPath = PathHelper.FindValidInstallationPath(FolderPath);
                    if (string.IsNullOrEmpty(ValidPath))
                    {
                        DialogService.ShowSimpleDialog("The specified path must include both the 'FortniteGame' and 'Engine' folders.", "Invalid Installation Path");
                        return;
                    }
                    FolderPath = ValidPath;
                }

                GlobalSettings.Options.FortnitePath = FolderPath;
                UserSettings.SaveSettings();

                Frame.Navigate(typeof(DownloadsPage), "Downloads");
            }
            catch (Exception Error)
            {
                DialogService.ShowSimpleDialog(Error.ToString(), "Change Install Path");
            }
        }

        private void DownloadBuild(object Sender, RoutedEventArgs Event)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = ProjectDefinitions.DownloadBuildURL,
                UseShellExecute = true
            });
        }

        private void VerifyBuild(object Sender, RoutedEventArgs Event)
        {
            DialogService.ShowSimpleDialog("This feature has been disabled while we work on a fix. If you experience any issues, please navigate to the support server for assistance.", "Updating");
        }
    }
}