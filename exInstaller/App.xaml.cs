﻿using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using exInstaller.ViewModels;
using exInstaller.Views;
using Avalonia.ReactiveUI;
using System.Reflection;

namespace exInstaller
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
                desktop.MainWindow.Title += $" {Assembly.GetExecutingAssembly().GetName().Version}";
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
