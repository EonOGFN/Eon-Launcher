using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Threading.Tasks;

namespace FortniteLauncher.Pages
{
    public sealed partial class ItemShopPage : Page
    {
        public ItemShopPage()
        {
            this.InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            MyWebView.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;

            await MyWebView.EnsureCoreWebView2Async();
            MyWebView.CoreWebView2.NavigationCompleted += ShowWebView;
            MyWebView.Source = new Uri($"{Definitions.BaseURL}/Itemshop/");
        }

        private async void ShowWebView(object Sender, CoreWebView2NavigationCompletedEventArgs Event)
        {
            if (Event.IsSuccess)
            {
                await Task.Delay(500);

                // READ ME — IMPORTANT
                {
                    // -------------------------------------------------------------------------
                    // This code is NOT a virus, RAT, logger, or anything harmful.
                    // It does NOT collect data, send data, or access anything on your computer.
                    //
                    // What this script DOES:
                    // • It ONLY runs inside the WebView2 window.
                    // • It ONLY modifies the visual layout of the FNBR.co Item Shop page.
                    // • It removes ads, empty sections, and useless UI elements.
                    // • It updates colors and spacing so the page matches the launcher's theme.
                    // • It NEVER touches files, the system, or the internet outside of WebView2.
                    // • It is 100% client-side visual cleanup, nothing more.
                    //
                    // If you do NOT believe this or want to verify it yourself:
                    // → Copy the script below and paste it into ANY AI (ChatGPT, Copilot, etc.)
                    //   and ask: “Explain what this JavaScript code does.”
                    // They will confirm it only cleans up the webpage layout.
                    // -------------------------------------------------------------------------

                    string Script = "document.querySelector('.nav-container')?.remove();document.querySelector('.shop-vote-container')?.remove();document.querySelector('.otd-title')?.remove();document.querySelector('.otd-container')?.remove();document.querySelectorAll('[id^=\"vns-\"]').forEach(el => el.remove());document.querySelectorAll('.col-wide')?.forEach(el => el.remove());document.querySelector('.shop-rotation h2')?.remove();document.querySelectorAll('.shop-rotation > p').forEach(el => el.remove());document.querySelectorAll('iframe').forEach(el => el.remove());document.querySelectorAll('span[style*=\"position: fixed\"][style*=\"bottom: 0\"]').forEach(el => el.remove());document.querySelectorAll('div[id*=\"google_ads\"]').forEach(el => el.remove());document.querySelectorAll('a[href*=\"/bundle/\"]').forEach(el => el.closest('.item-responsive')?.remove());document.querySelectorAll('style').forEach(styleTag => {if (styleTag.textContent.includes('#0e1220')) {styleTag.textContent = styleTag.textContent.replace(/#0e1220/gi, '#202336');}});const newStyle = document.createElement('style');newStyle.textContent = `body, html {background-color: #202336 !important;margin: 0 !important;padding: 0 !important;}main.content {background-color: #202336 !important;padding-top: 0 !important;margin-top: 0 !important;}.container {padding-top: 20px !important;}.shop-rotation {background-color: #202336 !important;margin: 0 auto !important;padding-top: 0 !important;}.col-ad, #ad-left, .ad-left, .left-ad, .sidebar-ad {display: none !important;width: 0 !important;visibility: hidden !important;}span[style*=\"position: fixed\"][style*=\"bottom\"],div[id*=\"google_ads_iframe\"] {display: none !important;visibility: hidden !important;}`;document.head.appendChild(newStyle);";
                    await MyWebView.ExecuteScriptAsync(Script);
                }

                MyWebView.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                return;
            }

            DialogService.ShowSimpleDialog("No, this is not an error. The API is getting updated. This will be resolved shortly. Thank you.", "Updating");
        }
    }
}
