using System;
using Windows.UI;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using System.Text.RegularExpressions;

class DialogService
{
    private const int IconSize = 40;
    private const int IconRadius = 20;
    private const int TitleFontSize = 20;
    private const int ContentFontSize = 14;
    private const int ContentLineHeight = 22;
    private const int ContentMaxWidth = 500;
    private const int DialogCornerRadius = 16;
    private const int ButtonCornerRadius = 8;

    public static async Task ShowSimpleDialog(string Content, string Title)
    {
        if (!ValidateWindow())
            return;

        await Processes.ForceCloseFortnite(true);

        string ProcessedContent = ProcessCustomErrors(Content, Title);
        ContentDialog Dialog = CreateDialog(Title, ProcessedContent, false);
        await Dialog.ShowAsync();
    }

    public static async Task<bool> YesOrNoDialog(string Content, string Title)
    {
        if (!ValidateWindow())
            return false;

        await Processes.ForceCloseFortnite(true);

        ContentDialog Dialog = CreateDialog(Title, Content, true);
        ContentDialogResult Result = await Dialog.ShowAsync();

        return Result == ContentDialogResult.Primary;
    }

    private static bool ValidateWindow()
    {
        return GlobalSettings.Windows?.Content?.XamlRoot != null;
    }

    private static ContentDialog CreateDialog(string Title, string Content, bool IsYesNo)
    {
        return new ContentDialog
        {
            XamlRoot = GlobalSettings.Windows.Content.XamlRoot,
            Title = CreateTitlePanel(Title),
            Content = CreateContentBlock(Content),
            PrimaryButtonText = IsYesNo ? "Yes" : null,
            CloseButtonText = IsYesNo ? "No" : "OK",
            DefaultButton = ContentDialogButton.Primary,
            Background = CreateSolidBrush(30, 30, 40),
            BorderBrush = CreateSolidBrush(60, 60, 70),
            CornerRadius = new CornerRadius(DialogCornerRadius),
            PrimaryButtonStyle = CreateButtonStyle(45, 45, 55, 200, 200, 200, 60, 60, 70),
            CloseButtonStyle = CreateButtonStyle(35, 35, 45, 160, 160, 170, 50, 50, 60)
        };
    }

    private static StackPanel CreateTitlePanel(string Title)
    {
        StackPanel Panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12
        };

        Panel.Children.Add(CreateIconGrid());
        Panel.Children.Add(CreateTitleTextBlock(Title));

        return Panel;
    }

    private static Grid CreateIconGrid()
    {
        Grid IconGrid = new Grid
        {
            Width = IconSize,
            Height = IconSize,
            CornerRadius = new CornerRadius(IconRadius),
            Background = CreateSolidBrush(239, 68, 68)
        };

        IconGrid.Children.Add(new TextBlock
        {
            Text = "!",
            FontSize = 24,
            FontWeight = new Windows.UI.Text.FontWeight { Weight = 700 },
            Foreground = CreateSolidBrush(255, 255, 255),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        });

        return IconGrid;
    }

    private static TextBlock CreateTitleTextBlock(string Title)
    {
        return new TextBlock
        {
            Text = Title,
            FontSize = TitleFontSize,
            FontWeight = new Windows.UI.Text.FontWeight { Weight = 700 },
            Foreground = CreateSolidBrush(255, 255, 255),
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    private static RichTextBlock CreateContentBlock(string Content)
    {
        RichTextBlock Block = new RichTextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            FontSize = ContentFontSize,
            Foreground = CreateSolidBrush(200, 200, 200),
            LineHeight = ContentLineHeight,
            MaxWidth = ContentMaxWidth
        };

        Paragraph Paragraph = new Paragraph();
        AddContentWithHyperlinks(Content, Paragraph);
        Block.Blocks.Add(Paragraph);

        return Block;
    }

    private static void AddContentWithHyperlinks(string Content, Paragraph Paragraph)
    {
        Regex UrlRegex = new Regex(@"https?://[^\s,]+", RegexOptions.IgnoreCase);
        MatchCollection Matches = UrlRegex.Matches(Content);

        int LastIndex = 0;

        foreach (Match Match in Matches)
        {
            if (Match.Index > LastIndex)
            {
                Paragraph.Inlines.Add(new Run { Text = Content.Substring(LastIndex, Match.Index - LastIndex) });
            }

            Hyperlink Link = new Hyperlink
            {
                NavigateUri = new Uri(Match.Value),
                Foreground = CreateSolidBrush(100, 150, 255),
                FontWeight = new Windows.UI.Text.FontWeight { Weight = 600 }
            };
            Link.Inlines.Add(new Run { Text = Match.Value });
            Paragraph.Inlines.Add(Link);

            LastIndex = Match.Index + Match.Length;
        }

        if (LastIndex < Content.Length)
        {
            Paragraph.Inlines.Add(new Run { Text = Content.Substring(LastIndex) });
        }
    }

    private static Style CreateButtonStyle(byte BackR, byte BackG, byte BackB, byte ForeR, byte ForeG, byte ForeB, byte BorderR, byte BorderG, byte BorderB)
    {
        Style ButtonStyle = new Style(typeof(Button));

        ButtonStyle.Setters.Add(new Setter(Button.BackgroundProperty, CreateSolidBrush(BackR, BackG, BackB)));
        ButtonStyle.Setters.Add(new Setter(Button.ForegroundProperty, CreateSolidBrush(ForeR, ForeG, ForeB)));
        ButtonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, CreateSolidBrush(BorderR, BorderG, BorderB)));
        ButtonStyle.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(1)));
        ButtonStyle.Setters.Add(new Setter(Button.CornerRadiusProperty, new CornerRadius(ButtonCornerRadius)));
        ButtonStyle.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(24, 10, 24, 10)));
        ButtonStyle.Setters.Add(new Setter(Button.FontWeightProperty, new Windows.UI.Text.FontWeight { Weight = 600 }));

        return ButtonStyle;
    }

    private static SolidColorBrush CreateSolidBrush(byte R, byte G, byte B)
    {
        return new SolidColorBrush(Color.FromArgb(255, R, G, B));
    }

    private static string ProcessCustomErrors(string Content, string Title)
    {
        if (Content.Contains("EasyAntiCheat"))
            return $"Easy Anti-Cheat needs to be reinstalled. Go to {GlobalSettings.Options.FortnitePath} and delete the EasyAntiCheat folder and Eon_EAC.exe file, then restart the launcher.";

        if (Content.Contains("because it is being used by another process"))
            return "Fortnite is already running. Close it and try again. If the issue persists, restart your computer.";

        if (Title.Contains("Corrupted Data Detected"))
            return $"Your game files are corrupted. Download the Fortnite build from {ProjectDefinitions.DownloadBuildURL}, extract it, and set the path in the launcher.";

        if (Content.Contains("SSL"))
            return "Connection issue detected. Download and enable CloudFlare WARP from https://one.one.one.one/ to continue.";

        return Content;
    }
}