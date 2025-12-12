using FortniteLauncher.Pages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using System;

class RequiredFilesDownloader
{
    private static readonly string[] DownloadMessages = new[]
    {
        "Almost done, please wait",
        "Almost ready to go",
        "Almost there, please wait",
        "Download in progress",
        "Downloading content",
        "Downloading essential files",
        "Downloading required files",
        "Getting things ready for you",
        "Nearly complete, please wait",
        "Nearly there, please wait",
        "Preparing your experience",
        "We're almost there",
        "You're almost ready",
        "You're just moments away",
    };

    private static readonly Random Randomizer = new();

    public static async Task Download()
    {
        ShowDownloadMessage();

        await PakChunk.EonPak();
        await PakChunk.BubbleBuilds();

        if (Definitions.bEnableEAC)
        {
            await EAC.DeleteFiles();
            await EAC.ExtractEAC();
        }

        await RedirectHandler.DownloadFile();
    }

    private static void ShowDownloadMessage()
    {
        string Message = DownloadMessages[Randomizer.Next(DownloadMessages.Length)];

        var MessageRun = new Run { Text = Message, FontSize = 14 };
        var MessageParagraph = new Paragraph();
        MessageParagraph.Inlines.Add(MessageRun);

        var MessageBlock = new RichTextBlock();
        MessageBlock.Blocks.Add(MessageParagraph);

        PlayPage.Launch_Button.Header = string.Empty;
        PlayPage.Launch_Button.Description = MessageBlock;

        StartLoadingAnimation(MessageRun, Message);
    }

    private static void StartLoadingAnimation(Run MessageRun, string BaseMessage)
    {
        int DotCount = 0;
        var Timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };

        Timer.Tick += (Sender, Event) =>
        {
            DotCount = (DotCount + 1) % 4;
            MessageRun.Text = BaseMessage + new string('.', DotCount);
        };

        Timer.Start();
    }
}