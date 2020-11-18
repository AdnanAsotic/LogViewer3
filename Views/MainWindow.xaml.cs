using LogViewer3.Behaviors;
using LogViewer3.ViewModels;
using ModernWpf.Controls;
using ModernWpf.Controls.Primitives;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace LogViewer3.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                foreach (var item in vm.Tabs)
                {
                    item.Cleanup();
                }
            }
        }
    }
}
