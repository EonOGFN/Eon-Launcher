using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using FortniteLauncher.Pages;
using WinUIEx;

namespace FortniteLauncher
{
    public sealed partial class MainWindow : WindowEx
    {
        public string LauncherName { get; } = $"{ProjectDefinitions.Name} Launcher";
        public static Frame ShellFrame { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            ConfigureWindow();
            ConfigureBackdrop();
            InitializeNavigation();
        }

        private void ConfigureWindow()
        {
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            this.SetWindowSize(1200, 725);
            this.CenterOnScreen();
            this.SetIcon("Content\\Texture\\Branding\\Eon.ico");
            Title = LauncherName;
        }

        private void ConfigureBackdrop()
        {
            SystemBackdrop = new MicaBackdrop
            {
                Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base
            };
        }

        private void InitializeNavigation()
        {
            ShellFrame = MainWindowFrame;
            MainWindowFrame.Navigate(typeof(LoginPage));
        }
    }
}