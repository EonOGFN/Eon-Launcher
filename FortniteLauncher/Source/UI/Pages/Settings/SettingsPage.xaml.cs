using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace FortniteLauncher.Pages
{
    public sealed partial class SettingsPage : Page
    {
        private readonly string LauncherVersion = GlobalSettings.Version;

        private readonly string About_Header = $"About {ProjectDefinitions.Name} Launcher";
        private readonly string GitHub_Juri = ProjectDefinitions.GitHub_Juri;
        private readonly string Github_David = ProjectDefinitions.GitHub_David;
        private readonly string Discord = ProjectDefinitions.Discord;
        private readonly string Tiktok = ProjectDefinitions.Tiktok;

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void PageLoaded(object Sender, RoutedEventArgs Event)
        {
            SoundToggle.IsOn = GlobalSettings.Options.IsSoundEnabled;
            WorldCupToggle.IsOn = GlobalSettings.Options.IsWCLobbyEnabled;
            BubbleBuildsToggle.IsOn = GlobalSettings.Options.IsBubbleBuildsEnabled;
        }

        private void ToggleSoundSwitch(object Sender, RoutedEventArgs Event)
        {
            var ToggleSwitch = (ToggleSwitch)Sender;
            if (ToggleSwitch.IsOn)
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
                GlobalSettings.Options.IsSoundEnabled = true;
            }
            else
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                GlobalSettings.Options.IsSoundEnabled = false;
            }

            UserSettings.SaveSettings();
        }
        private void ToggleWorldCup(object Sender, RoutedEventArgs Event)
        {
            if (WorldCupToggle.IsOn)
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
                GlobalSettings.Options.IsWCLobbyEnabled = true;
            }
            else
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                GlobalSettings.Options.IsWCLobbyEnabled = false;
            }

            UserSettings.SaveSettings();
        }

        private void ToggleBubbleBuilds(object Sender, RoutedEventArgs Event)
        {
            if (BubbleBuildsToggle.IsOn)
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
                GlobalSettings.Options.IsBubbleBuildsEnabled = true;
            }
            else
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                GlobalSettings.Options.IsBubbleBuildsEnabled = false;
            }

            UserSettings.SaveSettings();
        }

        private async void SignOutAccount(object Sender, RoutedEventArgs Event)
        {
            bool Result = await DialogService.YesOrNoDialog("This action will sign you out of your account. Your Account Information will be cleared from this device. Are you sure you want to proceed?", "Logging Out");

            if (Result)
            {
                GlobalSettings.Options.Email = string.Empty;
                GlobalSettings.Options.Password = string.Empty;
                UserSettings.SaveSettings();

                var Button = (Button)Sender;
                Button.IsEnabled = false;

                var ProgressRing = new ProgressRing
                {
                    IsIndeterminate = true,
                    Foreground = new SolidColorBrush(Colors.White)
                };

                Button.Content = ProgressRing;

                MainWindow.ShellFrame.Navigate(typeof(LoginPage));
                Button.IsEnabled = true;
            }
        }
    }
}