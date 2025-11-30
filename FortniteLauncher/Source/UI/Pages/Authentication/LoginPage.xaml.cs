using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;

namespace FortniteLauncher.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void LoginButton(object Sender, RoutedEventArgs Event)
        {
            bool IsEmailEmpty = string.IsNullOrWhiteSpace(MailBox.Text);
            bool IsPasswordEmpty = string.IsNullOrWhiteSpace(PasswordBox.Password);

            if (IsEmailEmpty || IsPasswordEmpty)
            {
                ShowAccessDeniedError("Email/Password is required.");
                return;
            }

            LoginBtn.IsEnabled = false;
            LoginBtn.Content = new ProgressRing
            {
                IsIndeterminate = true,
                Foreground = new SolidColorBrush(Microsoft.UI.Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            ApiResponse Response = await Authenticator.CheckLogin(MailBox.Text, PasswordBox.Password);

            switch (Response.Status)
            {
                case "Success":
                    if (Convert.ToBoolean(RememberMeCheckBox.IsChecked))
                    {
                        GlobalSettings.Options.Email = MailBox.Text;
                        GlobalSettings.Options.Password = PasswordBox.Password;
                        UserSettings.SaveSettings();
                    }
                    MainWindow.ShellFrame.Navigate(typeof(MainShellPage));
                break;

                default:
                LoginBtn.Content = "Login";
                LoginBtn.IsEnabled = true;
                break;
            }
        }

        private async void PageLoaded(object Sender, RoutedEventArgs Event)
        {
            if (Convert.ToBoolean(RememberMeCheckBox.IsChecked))
            {
                bool Email = !string.IsNullOrEmpty(GlobalSettings.Options.Email);
                bool Password = !string.IsNullOrEmpty(GlobalSettings.Options.Password);

                if (Email && Password)
                {
                    MailBox.Text = GlobalSettings.Options.Email;
                    PasswordBox.Password = GlobalSettings.Options.Password;

                    LoginBtn.IsEnabled = false;

                    LoginBtn.Content = new ProgressRing
                    {
                        IsIndeterminate = true,
                        Foreground = new SolidColorBrush(Microsoft.UI.Colors.Black),
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    ApiResponse LoginResponse = await Authenticator.CheckLogin(MailBox.Text, PasswordBox.Password);
                    string LoginStatus = LoginResponse.Status.ToString();

                    if (LoginStatus == "Success")
                    {
                        if (Convert.ToBoolean(RememberMeCheckBox.IsChecked))
                        {
                            GlobalSettings.Options.Email = MailBox.Text;
                            GlobalSettings.Options.Password = PasswordBox.Password;
                            UserSettings.SaveSettings();
                        }

                        MainWindow.ShellFrame.Navigate(typeof(MainShellPage));
                        return;
                    }

                    LoginBtn.IsEnabled = true;
                    LoginBtn.Content = "Login";
                }
            }
        }

        private void ShowAccessDeniedError(string Message)
        {
            StackPanel Panel = new StackPanel { Spacing = 2 };

            TextBlock Tittle = new TextBlock { Text = "Access Denied", FontWeight = FontWeights.Medium };
            TextBlock Description = new TextBlock { Text = Message };

            Panel.Children.Add(Tittle);
            Panel.Children.Add(Description);

            Grid g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            FontIcon Icon = new FontIcon { VerticalAlignment = VerticalAlignment.Center, Glyph = "\uE72E", FontSize = 28, Margin = new Thickness(0, 0, 12, 0) };

            g.Children.Add(Icon);
            g.Children.Add(Panel);

            Grid.SetColumn(Icon, 0);
            Grid.SetColumn(Panel, 1);

            ErrorNotification.Show(g, 2500);
        }

        private void Hyperlink(object Sender, RoutedEventArgs Event) => Windows.System.Launcher.LaunchUriAsync(((HyperlinkButton)Sender).NavigateUri);
    }
}