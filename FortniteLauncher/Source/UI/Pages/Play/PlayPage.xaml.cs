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
        private readonly string DisplayUsername = GetRandomGreeting();
        private readonly string Description = $"Experience the best Chapter {ProjectDefinitions.Chapter} Season {ProjectDefinitions.Season} experience with {ProjectDefinitions.Name}.";
        public static readonly string Season = "Launch Fortnite";
        public static readonly string Chapter = string.Empty;

        private static string GetRandomGreeting()
        {
            string Username = GlobalSettings.Options.Username;
            string[] Greetings = new[]
            {
                $"Hello, {Username}!",
                $"Welcome, {Username}!",
                $"Hey, {Username}!",
                $"What's up, {Username}!",
                $"Greetings, {Username}!",
                $"Hi, {Username}!",
                $"Howdy, {Username}!"
            };
            var Random = new Random();
            return Greetings[Random.Next(Greetings.Length)];
        }

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
            AnimateBlur();
        }

        private void AnimateBlur()
        {
            var Animation = new Storyboard();
            var ColorAnimation = new ColorAnimation
            {
                From = Windows.UI.Color.FromArgb(178, 0, 0, 0),
                To = Windows.UI.Color.FromArgb(204, 0, 0, 0),
                Duration = TimeSpan.FromMilliseconds(1250),
                EnableDependentAnimation = true,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(ColorAnimation, OverlayBrush);
            Storyboard.SetTargetProperty(ColorAnimation, "Color");
            Animation.Children.Add(ColorAnimation);
            Animation.Begin();
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
                await Task.Delay(5);
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
        private void Donations(object Sender, RoutedEventArgs EventArgs) => OpenUri(ProjectDefinitions.DonationsURL);
    }
}