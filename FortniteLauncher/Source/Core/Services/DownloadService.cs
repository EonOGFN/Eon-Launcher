using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

class DownloadService
{
    private static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler
    {
        MaxConnectionsPerServer = int.MaxValue
    })
    { Timeout = System.Threading.Timeout.InfiniteTimeSpan };

    public static string DownloadProgress { get; private set; } = "This may take several minutes, depending on your internet speed.";
    public static event Action<string> ProgressChanged;

    public static async Task File(string URL, string FilePath, string FileName)
    {
        try
        {
            string FullPath = Path.Combine(FilePath, FileName);

            using var Response = await HttpClient.GetAsync(URL, HttpCompletionOption.ResponseHeadersRead);
            Response.EnsureSuccessStatusCode();

            long TotalBytes = Response.Content.Headers.ContentLength ?? -1L;
            bool CanReportProgress = TotalBytes > 0;
            long DownloadedBytes = 0;

            await using var ContentStream = await Response.Content.ReadAsStreamAsync();
            await using var FileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write, FileShare.None);

            var Buffer = new byte[8192];
            int BytesRead;

            while ((BytesRead = await ContentStream.ReadAsync(Buffer)) > 0)
            {
                await FileStream.WriteAsync(Buffer.AsMemory(0, BytesRead));
                DownloadedBytes += BytesRead;

                if (CanReportProgress)
                {
                    double ProgressPercentage = (double)DownloadedBytes / TotalBytes * 100;
                    DownloadProgress = $"{FileName} Installation:\n{FormatSize(DownloadedBytes)} / {FormatSize(TotalBytes)} ({ProgressPercentage:F2}%)";
                    ProgressChanged?.Invoke(DownloadProgress);
                }
            }
        }
        catch (HttpRequestException Error)
        {
            ShowError(Error, FilePath, FileName);
        }
        catch (Exception Error)
        {
            ShowError(Error, FilePath, FileName);
        }
    }

    private static string FormatSize(long Bytes)
    {
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;

        if (Bytes < KB) return $"{Bytes} B";
        if (Bytes < MB) return $"{Bytes / (double)KB:F2} KB";
        if (Bytes < GB) return $"{Bytes / (double)MB:F2} MB";
        return $"{Bytes / (double)GB:F2} GB";
    }

    private static void ShowError(Exception Error, string FilePath, string FileName)
    {
        string Message = $"Error: {Error.Message}\nPath: {FilePath}\nFile: {FileName}\nFailed to install required files. Please check your internet connection and try again.";
        DialogService.ShowSimpleDialog(Message, "Error");
    }
}