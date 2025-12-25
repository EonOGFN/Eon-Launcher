using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Windows.Storage.Pickers;


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
            bool Confirmed = await DialogService.YesOrNoDialog(ConfirmationMessage, $"Deleting {ProjectDefinitions.Name}");

            if (!Confirmed)
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
                DialogService.ShowSimpleDialog($"An error occurred: {Exception.Message}", "Error");
            }
        }

        private async void ChangeInstallPath(object Sender, RoutedEventArgs EventArgs)
        {
            if (Sender is not Button Button)
                return;

            Button.IsEnabled = false;

            try
            {
                var Picker = new FolderPicker(Button.XamlRoot.ContentIslandEnvironment.AppWindowId);
                Picker.CommitButtonText = "Select Folder";
                Picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                Picker.ViewMode = PickerViewMode.Thumbnail;
                Picker.FileTypeFilter.Add("*");

                var SelectedFolder = await Picker.PickSingleFolderAsync();

                if (SelectedFolder == null)
                {
                    DialogService.ShowSimpleDialog("No folder was selected. Please select a valid installation folder.", "No Folder Selected");
                    return;
                }

                string FolderPath = SelectedFolder.Path;
                string[] CompressedExtensions = { ".rar", ".zip", ".7z" };

                if (CompressedExtensions.Any(Extension => FolderPath.EndsWith(Extension, StringComparison.OrdinalIgnoreCase)))
                {
                    DialogService.ShowSimpleDialog("The selected file appears to be compressed. Please extract it using a third party extraction tool.", "Compressed File Error");
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
                DialogService.ShowSimpleDialog($"An error occurred: {Exception.Message}", "Error");
            }
            finally
            {
                Button.IsEnabled = true;
            }
        }

        private void DownloadBuild(object Sender, RoutedEventArgs EventArgs)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = ProjectDefinitions.DownloadBuildURL,
                    UseShellExecute = true
                });
            }
            catch (Exception Exception)
            {
                DialogService.ShowSimpleDialog($"Failed to open download URL: {Exception.Message}", "Error");
            }
        }
    }
}