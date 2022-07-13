using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using exInstaller.ViewModels;
using exInstaller.Views;
using Avalonia.ReactiveUI;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System;

namespace exInstaller
{
    public class App : Application
    {
        private MainWindowViewModel mainWindowViewModel;
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                mainWindowViewModel = new MainWindowViewModel();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel,
                };
                desktop.MainWindow.Title += $" {Assembly.GetExecutingAssembly().GetName().Version}";
                desktop.ShutdownRequested += Desktop_ShutdownRequested;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void Desktop_ShutdownRequested(object sender, ShutdownRequestedEventArgs e)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    foreach (Process proc in Process.GetProcessesByName("arduino-cli"))
                    {
                        while (!proc.HasExited)
                        {
                            proc.Kill();
                        }
                    }
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    foreach (Process proc in Process.GetProcessesByName("arduino-cli.exe"))
                    {
                        while (!proc.HasExited)
                        {
                            proc.Kill();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
             
            }
        }
    }
}
