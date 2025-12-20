using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FortniteLauncher.Pages
{
    public sealed partial class MainShellPage : Page
    {
        public static NavigationView STATIC_MainNavigation;

        public MainShellPage()
        {
            this.InitializeComponent();
            NavigationService.InitializeNavigationService(MainNavigation, MainBreadcrumb, RootFrame);
            MainNavigation.SelectedItem = PlayPageItem;
        }

        private void MainNavigation_SelectionChanged(NavigationView Sender, NavigationViewSelectionChangedEventArgs Args)
        {
            if ((Args.SelectedItem as NavigationViewItem) == PlayPageItem) { NavigationService.Navigate(typeof(PlayPage), true); NavigationService.ChangeBreadcrumbVisibility(false); }
            if ((Args.SelectedItem as NavigationViewItem) == DownloadsItem) { NavigationService.Navigate(typeof(DownloadsPage), true); }
            if ((Args.SelectedItem as NavigationViewItem) == ItemShopItem) { NavigationService.Navigate(typeof(ItemShopPage), true); }
            if ((Args.SelectedItem as NavigationViewItem) == LeaderboardItem) { NavigationService.Navigate(typeof(LeaderboardPage), true); }
            if ((Args.SelectedItem as NavigationViewItem) == ServerStatusItem) { NavigationService.Navigate(typeof(ServerStatusPage), true); }
            if ((Args.SelectedItem as NavigationViewItem) == SettingsItem) { NavigationService.Navigate(typeof(SettingsPage), true); }
            ElementSoundPlayer.Play(ElementSoundKind.Invoke);
        }

        private void MainBreadcrumb_ItemClicked(BreadcrumbBar Sender, BreadcrumbBarItemClickedEventArgs Args)
        {
            if (Args.Index < NavigationService.BreadCrumbs.Count - 1)
            {
                var Crumb = (NavigationService.Breadcrumb)Args.Item;
                Crumb.NavigateToFromBreadcrumb(Args.Index);
            }
        }

        private void PaneContent_Click(object Sender, RoutedEventArgs Event)
        {
            MainNavigation.SelectedItem = null;
            NavigationService.Navigate(typeof(SettingsPage), true);
        }

        private void MainNavigation_Loaded(object Sender, RoutedEventArgs Event)
        {
            STATIC_MainNavigation = MainNavigation;
        }
    }
}