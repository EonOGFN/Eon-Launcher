using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FortniteLauncher.Pages;

class LaunchStatusService
{
    private const string CloseIcon = "\uE8BB";
    private const string LaunchIcon = "\uE768";
    private const string CloseFortniteText = "Close Fortnite";

    public static void OnGameOpened()
    {
        SetButtonState(CloseFortniteText, CloseIcon, isGameRunning: true);
    }

    public static void OnGameClosed(bool ForceClose = false)
    {
        if (ForceClose)
            Processes.ForceCloseFortnite();

        SetButtonState(PlayPage.Season, LaunchIcon, isGameRunning: false);
    }

    private static void SetButtonState(string Header, string Icon, bool isGameRunning)
    {
        PlayPage.Launch_Button.Click -= OnCloseButtonClick;

        PlayPage.Launch_Button.Header = Header;
        PlayPage.Launch_Button.Description = string.Empty;
        PlayPage.Launch_Button.Content = null;
        PlayPage.Launch_Button.HeaderIcon = new FontIcon { Glyph = Icon };

        Definitions.BindPlayButton = !isGameRunning;

        if (isGameRunning)
            PlayPage.Launch_Button.Click += OnCloseButtonClick;
    }

    private static void OnCloseButtonClick(object Sender, RoutedEventArgs Event)
    {
        OnGameClosed(ForceClose: true);
    }
}