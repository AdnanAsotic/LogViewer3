using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LogViewer3.Helpers
{
    public static class LoggerSymbols
    {
        public static SymbolIcon Document => new SymbolIcon(Symbol.Document);
        public static SymbolIcon Cancel => new SymbolIcon(Symbol.Cancel);
    }

    public static class TabItemHelper
    {
        #region Icon

        public static object GetIcon(TabItem tabItem)
        {
            return tabItem.GetValue(IconProperty);
        }

        public static void SetIcon(TabItem tabItem, object value)
        {
            tabItem.SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached(
                "Icon",
                typeof(object),
                typeof(TabItemHelper));

        #endregion
    }
}
