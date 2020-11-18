using ModernWpf.Controls;
using System.Windows;
using System.Windows.Input;

namespace LogViewer3.Behaviors
{
    public static class CallCommandBehavior
    {
        public static readonly DependencyProperty CommandProperty =
           DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(CallCommandBehavior), new PropertyMetadata(null, CommandChanged));

        public static void CommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            AutoSuggestBox autoSuggestBox = obj as AutoSuggestBox;

            if (autoSuggestBox != null)
            {
                autoSuggestBox.QuerySubmitted -= AutoSuggestBox_QuerySubmitted;
                autoSuggestBox.QuerySubmitted += AutoSuggestBox_QuerySubmitted;
            }
        }

        private static void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ICommand command = GetCommand(sender);
            command.Execute(args);
        }

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }
    }
}
