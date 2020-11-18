using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LogViewer3.Behaviors
{

    public static class AutoScrollBehavior
    {
        

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(AutoScrollBehavior), new PropertyMetadata(null, CommandChanged));

        public static void CommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            // not needed
        }

        public static bool GetCommand(DependencyObject obj)
        {
            return (bool)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty UndoCommandProperty =
            DependencyProperty.RegisterAttached("UndoCommand", typeof(ICommand), typeof(AutoScrollBehavior), new PropertyMetadata(null, CommandChanged));

        public static bool GetUndoCommand(DependencyObject obj)
        {
            return (bool)obj.GetValue(UndoCommandProperty);
        }

        public static void SetUndoCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(UndoCommandProperty, value);
        }

        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(AutoScrollBehavior), new PropertyMetadata(false, AutoScrollPropertyChanged));

        public static void AutoScrollPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var scrollViewer = obj as ScrollViewer;

            if (scrollViewer != null && (bool)args.NewValue)
            {
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                scrollViewer.ScrollToEnd();
            }
            else
            {
                Undo(scrollViewer, null);
                scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
            }
        }

        private static bool _scrolled = false;

        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_scrolled)
            {
                _scrolled = false;
            }
            else if (e.ExtentHeightChange == 0)
            {
                ScrollViewer sw = sender as ScrollViewer;
                
                if (e.VerticalOffset == sw.ScrollableHeight)
                {
                    Do(sender, e);
                }
                else if (e.VerticalChange != 0)
                {
                    Undo(sender, e);
                }
            }
            
            // Only scroll to bottom when the extent changed. Otherwise you can't scroll up
            if (e.ExtentHeightChange != 0)
            {
                var scrollViewer = sender as ScrollViewer;
                _scrolled = true;
                scrollViewer?.ScrollToBottom();
            }
        }

        private static void Undo(object sender, ScrollChangedEventArgs e)
        {
            ICommand command = (sender as UIElement).GetValue(UndoCommandProperty) as ICommand;

            if (command == null) return;

            if (command.CanExecute(e))
            {
                command.Execute(e);
            }
        }

        private static void Do(object sender, ScrollChangedEventArgs e)
        {
            ICommand command = (sender as UIElement).GetValue(CommandProperty) as ICommand;
            
            if (command == null) return;

            if (command.CanExecute(e))
            {
                command?.Execute(e);
            }
        }

        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }
    }
}
