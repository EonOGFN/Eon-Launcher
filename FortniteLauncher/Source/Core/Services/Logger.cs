using System;
using System.IO;
using System.Threading.Tasks;

class Logger
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "launcher.log");

        public static async Task LogAsync(string message, string level = "INFO")
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logEntry = $"[{timestamp}] [{level}] {message}{Environment.NewLine}";

                await File.AppendAllTextAsync(LogFilePath, logEntry);
            }
            catch (Exception ex)
            {
                // Fallback to console if file logging fails
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }

        public static async Task LogErrorAsync(string message, Exception ex = null)
        {
            string fullMessage = ex != null ? $"{message}: {ex.Message}" : message;
            await LogAsync(fullMessage, "ERROR");
        }

        public static async Task LogWarningAsync(string message)
        {
            await LogAsync(message, "WARNING");
        }

        public static async Task LogInfoAsync(string message)
        {
            await LogAsync(message, "INFO");
        }
    }