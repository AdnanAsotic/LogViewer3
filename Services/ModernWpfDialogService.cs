using ModernWpf.Controls;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System;
using System.Linq;
using System.Windows;

namespace LogViewer3.Services
{
    public class ModernWpfDialogService : DialogService
    {
        public ModernWpfDialogService(IContainerExtension containerExtension) : base(containerExtension)
        {
        }

        protected override void ConfigureDialogWindowProperties(IDialogWindow window, FrameworkElement dialogContent, IDialogAware viewModel)
        {
            if (window is ContentDialog contentDialog && dialogContent is ContentDialog wrappedDialog)
            {
                var windowStyle = Dialog.GetWindowStyle(dialogContent);
                if (windowStyle != null)
                    window.Style = windowStyle;

                contentDialog.Title = wrappedDialog.Title;
                contentDialog.PrimaryButtonText = wrappedDialog.PrimaryButtonText;
                contentDialog.SecondaryButtonText = wrappedDialog.SecondaryButtonText;
                contentDialog.SecondaryButtonCommand = wrappedDialog.SecondaryButtonCommand;
                contentDialog.SecondaryButtonCommandParameter = wrappedDialog.SecondaryButtonCommandParameter;
                contentDialog.PrimaryButtonCommand = wrappedDialog.PrimaryButtonCommand;
                contentDialog.PrimaryButtonCommandParameter = wrappedDialog.PrimaryButtonCommandParameter;
                contentDialog.DefaultButton = wrappedDialog.DefaultButton;
                contentDialog.CloseButtonText = wrappedDialog.CloseButtonText;
                contentDialog.CloseButtonCommand = wrappedDialog.CloseButtonCommand;
                contentDialog.CloseButtonCommandParameter = wrappedDialog.CloseButtonCommandParameter;

                contentDialog.Content = wrappedDialog.Content;
                contentDialog.DataContext = viewModel;

                if (contentDialog.Owner == null)
                    contentDialog.Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            }
            else
            {
                base.ConfigureDialogWindowProperties(window, dialogContent, viewModel);
            }
        }
    }
}
