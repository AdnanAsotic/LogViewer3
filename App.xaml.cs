using Dragablz;
using LogViewer3.Services;
using LogViewer3.Views;
using ModernWpf.Controls;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Services.Dialogs;
using System;
using System.ComponentModel;
using System.Windows;

namespace LogViewer3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDialogService, ModernWpfDialogService>();
            containerRegistry.RegisterSingleton<IOsDialogService, OsDialogService>();
            containerRegistry.RegisterSingleton<IConfigurationService, ConfigurationService>();
            containerRegistry.RegisterDialog<AutoLoadConfigDialog>();
            containerRegistry.Register<IDialogWindow, ModernWpfDialogWindow>();
            containerRegistry.RegisterSingleton<IInterTabClient, LogViewerInterTabClient>();
            containerRegistry.Register<MainWindow, MainWindow>();
        }
    }

   
}
