using CommunityToolkit.Labs.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FortniteLauncher.Pages
{
    public sealed partial class PlayPage : Page
    {
        public static SettingsCard Launch_Button;
        public static ProgressRing ProgressRing;
        private string Progress = DownloadService.DownloadProgress;
        private readonly string DisplayUsername = $"Welcome, {GlobalSettings.Options.Username}!";
        private readonly string Description = $"Relive the best Chapter {ProjectDefinitions.Chapter} Season {ProjectDefinitions.Season} experience with {ProjectDefinitions.Name}.";
        public static readonly string Season = "Launch Fortnite";
        public static readonly string Chapter = string.Empty;

        public PlayPage()
        {
            InitializeComponent();
            LoadProfileImage();
            Launch_Button = LaunchButton;
            DownloadService.ProgressChanged += OnDownloadProgressChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs EventArgs)
        {
            base.OnNavigatedTo(EventArgs);
            HeaderBlurBorder.Opacity = 0.7;

            var Storyboard = new Storyboard();
            var FadeAnimation = new DoubleAnimation
            {
                From = 0.7,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(1250),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTarget(FadeAnimation, HeaderBlurBorder);
            Storyboard.SetTargetProperty(FadeAnimation, "Opacity");
            Storyboard.Children.Add(FadeAnimation);
            Storyboard.Begin();
        }

        private async void Launch(object Sender, RoutedEventArgs EventArgs)
        {
            if (!Definitions.BindPlayButton)
                return;

            if (!PathHelper.IsPathValid(GlobalSettings.Options.FortnitePath))
            {
                DialogService.ShowSimpleDialog("Fortnite path not found. Please check your installation and try again.", "Invalid Fortnite Path");
                UI.ShowProgressRing((SettingsCard)Sender, false);
                return;
            }

            ShowDownloadProgress();
            UI.ShowProgressRing((SettingsCard)Sender, true);
            await Processes.ForceCloseFortnite();
            await Fortnite.Launch(GlobalSettings.Options.FortnitePath);
            DownloadInfo.IsOpen = false;
        }

        private void OnDownloadProgressChanged(string DownloadStatus)
        {
            Progress = DownloadStatus;
            DispatcherQueue.TryEnqueue(() => DownloadInfo.SetValue(TeachingTip.SubtitleProperty, DownloadStatus));
        }

        private async void ShowDownloadProgress()
        {
            DownloadInfo.IsOpen = true;
            while (DownloadInfo.IsOpen)
            {
                DispatcherQueue.TryEnqueue(() => DownloadInfo.Subtitle = DownloadService.DownloadProgress);
                await Task.Delay(20);
            }
            DownloadInfo.IsOpen = false;
        }

        private void LoadProfileImage()
        {
            var URL = GlobalSettings.Options.SkinUrl;
            if (!string.IsNullOrEmpty(URL))
                ProfileImageBrush.ImageSource = new BitmapImage(new Uri(URL, UriKind.Absolute));
        }

        private void OpenUri(string URI) => Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = URI });

        private void Tiktok(object Sender, RoutedEventArgs EventArgs) => OpenUri(ProjectDefinitions.Tiktok);

        private void Discord(object Sender, RoutedEventArgs EventArgs) => OpenUri(ProjectDefinitions.Discord);
    }
}