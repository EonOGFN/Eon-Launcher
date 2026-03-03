using FortniteLauncher.Pages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

class RequiredFilesDownloader
{
    private static readonly Random Randomizer = new();

    public static async Task Download()
    {
        ShowDownloadMessage();

        // Change DNS to 1.1.1.1 for faster downloads
        await ChangeDnsForDownloads();

        // Parallel downloads where possible
        var pakTasks = new[]
        {
            PakChunk.EonPak(),
            PakChunk.BubbleBuilds()
        };
        await Task.WhenAll(pakTasks);

        await EAC.Execute(EACOperation.Installation);

        await RedirectHandler.DownloadFile();
        await D3DCompilerCheck.DownloadFile();
    }

    private static async Task ChangeDnsForDownloads()
    {
        try
        {
            await Logger.LogInfoAsync("Changing DNS to 1.1.1.1 for faster downloads");

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = "interface ip set dns \"Ethernet\" static 1.1.1.1",
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = true
            };

            Process process = Process.Start(psi);
            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                await Logger.LogInfoAsync("DNS changed to 1.1.1.1 successfully for downloads");
            }
            else
            {
                await Logger.LogWarningAsync("Failed to change DNS automatically for downloads");
            }
        }
        catch (Exception ex)
        {
            await Logger.LogErrorAsync("Exception while changing DNS for downloads", ex);
        }
    }

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