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
    { Timeout = TimeSpan.FromMinutes(10) }; // Reasonable timeout instead of infinite

    public static string DownloadProgress { get; private set; } = "This may take several minutes, depending on your internet speed.";
    public static event Action<string> ProgressChanged;

    public static async Task DownloadFile(string URL, string FilePath, string FileName, int maxRetries = 3, bool useCache = false)
    {
        string FullPath = Path.Combine(FilePath, FileName);

        // Simple caching: skip if file exists and useCache is true
        if (useCache && File.Exists(FullPath))
        {
            await Logger.LogInfoAsync($"File already exists, skipping download: {FileName}");
            return;
        }

        int retryCount = 0;

        while (retryCount <= maxRetries)
        {
            try
            {
                await Logger.LogInfoAsync($"Starting download: {FileName} from {URL}");

                using var Response = await HttpClient.GetAsync(URL, HttpCompletionOption.ResponseHeadersRead);
                Response.EnsureSuccessStatusCode();

                long TotalBytes = Response.Content.Headers.ContentLength ?? -1L;
                bool CanReportProgress = TotalBytes > 0;
                long DownloadedBytes = 0;

                await using var ContentStream = await Response.Content.ReadAsStreamAsync();
                await using var FileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write, FileShare.None);

                var Buffer = new byte[65536]; // Increased buffer size for faster downloads
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

                await Logger.LogInfoAsync($"Download completed: {FileName}");
                return;
            }
            catch (HttpRequestException Error)
            {
                retryCount++;
                if (retryCount > maxRetries)
                {
                    await Logger.LogErrorAsync($"Download failed after {maxRetries} retries: {FileName}", Error);
                    ShowError(Error, FilePath, FileName);
                    return;
                }
                await Logger.LogWarningAsync($"Download retry {retryCount} for {FileName}: {Error.Message}");
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount))); // Exponential backoff
            }
            catch (Exception Error)
            {
                await Logger.LogErrorAsync($"Unexpected error during download: {FileName}", Error);
                ShowError(Error, FilePath, FileName);
                return;
            }
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