using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

class DownloadService
{
    private const int BufferSize = 32768;
    private const int TimeoutMinutes = 60;

    private static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler
    {
        MaxConnectionsPerServer = int.MaxValue,
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    })
    { Timeout = TimeSpan.FromMinutes(TimeoutMinutes) };

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

            var Buffer = new byte[BufferSize];
            int BytesRead;

            while ((BytesRead = await ContentStream.ReadAsync(Buffer, 0, Buffer.Length)) > 0)
            {
                await FileStream.WriteAsync(Buffer, 0, BytesRead);
                DownloadedBytes += BytesRead;

                if (CanReportProgress)
                {
                    double ProgressPercentage = (double)DownloadedBytes / TotalBytes * 100;
                    DownloadProgress = $"{FileName} Installation:\n{FormatSize(DownloadedBytes)} / {FormatSize(TotalBytes)} ({ProgressPercentage:F2}%)";
                    ProgressChanged?.Invoke(DownloadProgress);
                }
            }
        }
        catch (TaskCanceledException Error) when (Error.InnerException is TimeoutException)
        {
            DialogService.ShowSimpleDialog("Download exceeded over 60 minutes. Please check your internet connection and try again.", "Try Again!");
        }
        catch (HttpRequestException Error)
        {
            ShowError(Error, FilePath, FileName, "HttpRequestException");
        }
        catch (Exception Error)
        {
            ShowError(Error, FilePath, FileName, "Exception");
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

    private static void ShowError(Exception Error, string FilePath, string FileName, string ErrorType)
    {
        string Message = $"Error: {Error.Message}\nPath: {FilePath}\nFile: {FileName}\nFailed to install required files. Please check your internet connection and try again.";
        DialogService.ShowSimpleDialog(Message, $"Error ({ErrorType})");
    }
}