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
        private string Season = $"{ProjectDefinitions.Name} Build ({ProjectDefinitions.Build}-CL-{ProjectDefinitions.ContentLevel})";
        private string Install = $"Install {ProjectDefinitions.Name}";
        private string InstallBody = $"Download the {ProjectDefinitions.Build} Fortnite Build, essential for playing {ProjectDefinitions.Name}.";
        private string UninstallHeader = $"Uninstall {ProjectDefinitions.Name}";
        private string UninstallBody = $"Delete Chapter {ProjectDefinitions.Chapter} Season {ProjectDefinitions.Season} from your computer. This will not uninstall the {ProjectDefinitions.Name} Launcher.";

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

        private async void DeleteBuild(object Sender, RoutedEventArgs EventArgs)
        {
            string ConfirmationMessage = $"Are you sure you want to remove Fortnite Version {ProjectDefinitions.Build} from your computer? This action cannot be undone.";
            bool bConfirmed = await DialogService.YesOrNoDialog(ConfirmationMessage, $"Deleting {ProjectDefinitions.Name}");

            if (!bConfirmed)
            {
                DialogService.ShowSimpleDialog($"Your request to remove Fortnite Version {ProjectDefinitions.Build} has been canceled. No changes were made.", "Cancellation Confirmed");
                return;
            }

            try
            {
                if (!Directory.Exists(GlobalSettings.Options.FortnitePath))
                {
                    DialogService.ShowSimpleDialog("Could not find the Fortnite Version at the specified location.", "Not Found");
                    return;
                }

                Directory.Delete(GlobalSettings.Options.FortnitePath, true);
                DialogService.ShowSimpleDialog($"{ProjectDefinitions.Name} has been successfully removed from your computer.", "Removal Confirmation");
            }
            catch (Exception Exception)
            {
                DialogService.ShowSimpleDialog($"{Exception.Message}", "An error occurred please report this to a moderator.");
            }
        }

        private async void ChangeInstallPath(object Sender, RoutedEventArgs EventArgs)
        {
            try
            {
                FolderPicker Picker = new FolderPicker
                {
                    ViewMode = PickerViewMode.Thumbnail
                };
                Picker.FileTypeFilter.Add("*");

                if (GlobalSettings.Windows == null)
                {
                    DialogService.ShowSimpleDialog("Window reference is null.", "Error");
                    return;
                }

                IntPtr WindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(GlobalSettings.Windows);
                WinRT.Interop.InitializeWithWindow.Initialize(Picker, WindowHandle);

                var SelectedFolder = await Picker.PickSingleFolderAsync();
                if (SelectedFolder == null)
                {
                    DialogService.ShowSimpleDialog("No folder was selected. Please select a valid installation folder.", "No Folder Selected");
                    return;
                }

                string FolderPath = SelectedFolder.Path;
                string[] CompressedExtensions = { ".rar", ".zip" };

                if (CompressedExtensions.Any(Extension => FolderPath.EndsWith(Extension, StringComparison.OrdinalIgnoreCase)))
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
            catch (Exception Exception)
            {
                DialogService.ShowSimpleDialog(Exception.ToString(), "Change Install Path");
            }
        }

        private void DownloadBuild(object Sender, RoutedEventArgs EventArgs)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = ProjectDefinitions.DownloadBuildURL,
                UseShellExecute = true
            });
        }
    }
}