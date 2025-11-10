using Microsoft.UI.Xaml;
using System;
using System.Threading;

namespace FortniteLauncher
{
    public partial class App : Application
    {
        private Mutex MutexInstance;
        private Window MainWindowInstance;

        public App()
        {
            try
            {
                InitializeComponent();
                EnsureSingleInstance();

                EonRPC.Start();
                Processes.ForceCloseFortnite();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Woah, there's an error: {ex.Message}");
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs Arguments)
        {
            try
            {
                InitializeMainWindow();
                ConfigureSettings();
            }
            catch (Exception Error)
            {
                MessageBox.Show($"Report this error to a Moderator: {Error.Message}", "Error");
            }
        }

        private void EnsureSingleInstance()
        {
            MutexInstance = new Mutex(true, ProjectDefinitions.Name, out bool CreatedNew);

            if (CreatedNew)
            {
                MutexInstance.ReleaseMutex();
                return;
            }

            MessageBox.Show($"{ProjectDefinitions.Name} Launcher is already running. Please close it before opening a new instance.", "Already Running");
            Environment.Exit(1);
        }

        private void InitializeMainWindow()
        {
            MainWindowInstance = new MainWindow();
            MainWindowInstance.Activate();
            GlobalSettings.Windows = MainWindowInstance;
        }

        private void ConfigureSettings()
        {
            UserSettings.LoadSettings();

            if (GlobalSettings.Options.IsSoundEnabled)
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
            }
        }
    }
}