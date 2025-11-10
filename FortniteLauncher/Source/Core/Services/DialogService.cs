using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Documents;
using System;

class DialogService
{
    public static async Task ShowSimpleDialog(string Content, string Title)
    {
        if (GlobalSettings.Windows?.Content?.XamlRoot == null)
            return;

        await Processes.ForceCloseFortnite(true);

        string DialogContent = ProcessCustomErrors(Content, Title);
        bool IsUri = CheckIfUri(DialogContent);

        ContentDialog Dialog = new ContentDialog
        {
            XamlRoot = GlobalSettings.Windows.Content.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = Title,
            CloseButtonText = "OK",
            Content = CreateTextBlock(DialogContent, IsUri)
        };

        await Dialog.ShowAsync();
    }

    public static async Task<bool> YesOrNoDialog(string Content, string Title)
    {
        if (GlobalSettings.Windows?.Content?.XamlRoot == null)
            return false;

        await Processes.ForceCloseFortnite(true);

        ContentDialog Dialog = new ContentDialog
        {
            XamlRoot = GlobalSettings.Windows.Content.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = Title,
            PrimaryButtonText = "Yes",
            CloseButtonText = "No",
            Content = CreateTextBlock(Content, false)
        };

        ContentDialogResult Result = await Dialog.ShowAsync();
        return Result == ContentDialogResult.Primary;
    }

    private static TextBlock CreateTextBlock(string Content, bool IsUri)
    {
        TextBlock TextBlock = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
        };

        if (IsUri)
        {
            TextBlock.Inlines.Add(new Run { Text = $"We have encountered an issue with downloading the required files to continue playing {ProjectDefinitions.Name}. Please download the " });
            TextBlock.Inlines.Add(new Hyperlink
            {
                NavigateUri = new Uri("https://one.one.one.one/"),
                Inlines = { new Run { Text = "CloudFlare Warp VPN" } }
            });
            TextBlock.Inlines.Add(new Run { Text = " and activate it to continue playing." });
        }
        else
        {
            TextBlock.Text = Content;
        }

        return TextBlock;
    }

    private static string ProcessCustomErrors(string Content, string Title)
    {
        if (Content.Contains("EasyAntiCheat"))
            return $"There seems to be an issue with Easy Anti-Cheat. To fix this, navigate to {GlobalSettings.Options.FortnitePath} and locate the following folder and file:\n\nFolder: EasyAntiCheat\nFile: Eon_EAC.exe\n\nDelete both, then restart the launcher and try launching the game again to ensure the issue is resolved.";

        if (Content.Contains("because it is being used by another process."))
            return "Oops! Fortnite is already running or used by something else. Please try to launch again or find out where it's being used. If that doesn't work, restart your PC.";

        if (Title.Contains("Corrupted Data Detected"))
            return $"We've found damaged files, which is preventing online play.\n\nTo resolve this issue:\n1. Join the official {ProjectDefinitions.Name} Discord server\n2. Download Chapter {ProjectDefinitions.Chapter}, Season {ProjectDefinitions.Season} (Build {ProjectDefinitions.Build})\n3. Extract the files and set the install path in the {ProjectDefinitions.Name} Launcher\n\nIf you need help, feel free to contact our support team on Discord.";

        return Content;
    }

    private static bool CheckIfUri(string Content)
    {
        if (Content.Contains("SSL"))
            return true;

        Uri Result;
        return Uri.TryCreate(Content, UriKind.Absolute, out Result) && (Result.Scheme == Uri.UriSchemeHttp || Result.Scheme == Uri.UriSchemeHttps);
    }
}