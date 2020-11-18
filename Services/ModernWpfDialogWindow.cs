using System;
using System.ComponentModel;
using ModernWpf.Controls;
using Prism.Services.Dialogs;

namespace LogViewer3.Services
{
    public class ModernWpfDialogWindow : ContentDialog, IDialogWindow
    {
        public ModernWpfDialogWindow()
        {
            Closed += ModernDialogWindow_Closed;
            Closing += ModernDialogWindow_Closing;
        }

        public IDialogResult Result { get; set; }

        event EventHandler IDialogWindow.Closed
        {
            add
            {
                innerClosedEvent += value;
            }

            remove
            {
                innerClosedEvent -= value;
            }
        }

     
        event CancelEventHandler IDialogWindow.Closing
        {
            add
            {
                innerClosingEvent += value;
            }

            remove
            {
                innerClosingEvent -= value;
            }
        }

        EventHandler innerClosedEvent;
        CancelEventHandler innerClosingEvent;

        private void ModernDialogWindow_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            innerClosedEvent.Invoke(sender, null);
        }

        private void ModernDialogWindow_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            var cancelArgs = new CancelEventArgs();
            innerClosingEvent.Invoke(sender, cancelArgs);
            args.Cancel = cancelArgs.Cancel;
        }

        public void Close()
        {
            Hide();
        }

        public void Show()
        {
            base.ShowAsync();
        }

        public bool? ShowDialog()
        {
            base.ShowAsync();
            return true;
        }
    }
}
