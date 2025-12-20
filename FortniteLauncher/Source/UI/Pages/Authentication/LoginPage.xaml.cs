using System;
using System.IO;
using System.Text.Json;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

namespace FortniteLauncher.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void PageLoaded(object Sender, RoutedEventArgs EventArgs)
        {
            try
            {
                await LoginWebView.EnsureCoreWebView2Async();

                string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Source", "UI", "Pages", "Authentication", "Public");
                string HtmlPath = Path.Combine(BasePath, "LoginPage.html");
                string CssPath = Path.Combine(BasePath, "LoginPage.css");
                string JsPath = Path.Combine(BasePath, "LoginPage.js");

                if (File.Exists(HtmlPath) && File.Exists(CssPath) && File.Exists(JsPath))
                {
                    string HtmlContent = File.ReadAllText(HtmlPath);
                    string CssContent = File.ReadAllText(CssPath);
                    string JsContent = File.ReadAllText(JsPath);

                    string CombinedHtml = HtmlContent
                        .Replace("<link rel=\"stylesheet\" href=\"LoginPage.css\">", $"<style>{CssContent}</style>")
                        .Replace("<script src=\"LoginPage.js\"></script>", $"<script>{JsContent}</script>");

                    LoginWebView.NavigateToString(CombinedHtml);
                }

                LoginWebView.CoreWebView2.WebMessageReceived += MessageReceived;
            }
            catch (Exception Exception)
            {
                DialogService.ShowSimpleDialog($"Error loading WebView2: {Exception.Message}", "Error");
            }
        }

        private async void MessageReceived(CoreWebView2 Sender, CoreWebView2WebMessageReceivedEventArgs Args)
        {
            try
            {
                var Message = JsonSerializer.Deserialize<LoginMessage>(Args.WebMessageAsJson);

                if (Message?.Action == "CheckCredentials")
                {
                    await CheckCredentials();
                }
                else if (Message?.Action == "Login")
                {
                    await HandleLogin(Message);
                }
            }
            catch (Exception Exception)
            {
                DialogService.ShowSimpleDialog($"Error handling message: {Exception.Message}", "Error");
                await SendMessageToWebView(new
                {
                    Status = "Error",
                    Title = "Error",
                    Message = Exception.Message
                });
            }
        }

        private async Task CheckCredentials()
        {
            if (!string.IsNullOrEmpty(GlobalSettings.Options.Email) && !string.IsNullOrEmpty(GlobalSettings.Options.Password))
            {
                ApiResponse Response = await Authenticator.CheckLogin(GlobalSettings.Options.Email, GlobalSettings.Options.Password);

                await SendMessageToWebView(new
                {
                    Action = "AutoLogin",
                    Status = Response.Status,
                    Username = GlobalSettings.Options.Username ?? "Player",
                    SkinUrl = GlobalSettings.Options.SkinUrl ?? "https://cdn.eonfn.dev/EonS17.png",
                    DownloadUrl = ProjectDefinitions.DownloadBuildURL
                });

                if (Response.Status == "Success")
                {
                    await Task.Delay(2000);
                    MainWindow.ShellFrame.Navigate(typeof(MainShellPage));
                }
                return;
            }

            await SendMessageToWebView(new { Action = "ShowLogin" });
        }

        private async Task HandleLogin(LoginMessage Message)
        {
            ApiResponse Response = await Authenticator.CheckLogin(Message.Email, Message.Password);

            if (Response.Status == "Success")
            {
                GlobalSettings.Options.Email = Message.Email;
                GlobalSettings.Options.Password = Message.Password;

                if (Message.RememberMe)
                {
                    UserSettings.SaveSettings();
                }
            }

            await SendMessageToWebView(new
            {
                Action = "LoginResponse",
                Status = Response.Status,
                Username = GlobalSettings.Options.Username ?? "Player",
                SkinUrl = GlobalSettings.Options.SkinUrl ?? "https://cdn.eonfn.dev/EonS17.png",
                DownloadUrl = ProjectDefinitions.DownloadBuildURL
            });

            if (Response.Status == "Success")
            {
                await Task.Delay(2000);
                MainWindow.ShellFrame.Navigate(typeof(MainShellPage));
            }
        }

        private async Task SendMessageToWebView(object Data)
        {
            try
            {
                string Json = JsonSerializer.Serialize(Data);
                string Script = 
                $@"
                    if (window.chrome && window.chrome.webview) {{
                        window.dispatchEvent(new MessageEvent('message', {{ 
                            data: {Json} 
                        }}));
                    }}
                ";
                await LoginWebView.CoreWebView2.ExecuteScriptAsync(Script);
            }
            catch (Exception Exception)
            {
                DialogService.ShowSimpleDialog($"Error sending message to WebView: {Exception.Message}", "Error");
            }
        }

        private class LoginMessage
        {
            public string Action { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public bool RememberMe { get; set; }
        }
    }
}