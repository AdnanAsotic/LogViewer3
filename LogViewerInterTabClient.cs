using Dragablz;
using LogViewer3.Views;
using Prism.Ioc;
using System.Windows;

namespace LogViewer3
{
    public class LogViewerInterTabClient : IInterTabClient
    {
        public LogViewerInterTabClient(IContainerExtension containerExtension)
        {
            ContainerExtension = containerExtension;
        }

        public IContainerExtension ContainerExtension { get; }

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var view = ContainerExtension.Resolve<MainWindow>();
            
            return new NewTabHost<Window>(view, view.InitialTabablzControl);
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return TabEmptiedResponse.CloseWindowOrLayoutBranch;
        }
    }
}
