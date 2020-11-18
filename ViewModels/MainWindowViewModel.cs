using Dragablz;
using Dragablz.Dockablz;
using LogViewer3.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace LogViewer3.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public static bool AutoLoadDone = false;

        private string _title = "LogViewer3";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private IInterTabClient _interTabClient;
        public IOsDialogService OsDialogService { get; }
        public IDialogService DialogService { get; }
        public IConfigurationService ConfigurationService { get; }
        public ObservableCollection<TabViewModel> Tabs { get; set; } = new ObservableCollection<TabViewModel>();

        public ICommand ShowFileDialogCommand { get; set; }
        public IInterTabClient InterTabClient
        {
            get { return _interTabClient; }
        }
        public MainWindowViewModel(IOsDialogService osDialogService, 
            IDialogService dialogService, 
            IConfigurationService configurationService, 
            IInterTabClient client)
        {
            _interTabClient = client;
            OsDialogService = osDialogService ?? throw new System.ArgumentNullException(nameof(osDialogService));
            DialogService = dialogService;
            ConfigurationService = configurationService;


            ShowFileDialogCommand = new DelegateCommand(() => {
                string filePath = OsDialogService.ShowFileDialog(@"C:\Users\Adnan.Asotic\Documents\Logs").FirstOrDefault();
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    Tabs.Add(CreateTab(filePath));
                    
                    RaisePropertyChanged("HasTabs");
                    RaisePropertyChanged("HasNoTabs");
                }
            });

            ShowAutoLoadConfigurationCommand = new DelegateCommand(() => {
                DialogService.ShowDialog("AutoLoadConfigDialog", null, (r) => { });
            });

            if (!AutoLoadDone)
            {
                foreach (var load in ConfigurationService.GetAutoLoads())
                {
                    Tabs.Add(CreateTab(load.Path, true));
                }
                
                RaisePropertyChanged("HasTabs");
                RaisePropertyChanged("HasNoTabs");

                AutoLoadDone = true;
            }
        }

        private TabViewModel _selectedTab;
        public TabViewModel SelectedTab 
        {
            get => _selectedTab;
            set
            {
                SetProperty(ref _selectedTab, value);
                _selectedTab?.Refresh();
            }
        }

        private TabViewModel CreateTab(string path, bool loadOnStartup = false)
        {
            TabViewModel tab = new TabViewModel(ConfigurationService, loadOnStartup);
            tab.FilePath = path;

            SelectedTab = tab;

            return tab;
        }

        public ICommand ShowAutoLoadConfigurationCommand { get; set; }

        public static Guid TabPartition
        {
            get { return new Guid("2AE89D18-F236-4D20-9605-6C03319038E6"); }
        }

        public ItemActionCallback ClosingTabItemHandler
        {
            get { return ClosingTabItemHandlerImpl; }
        }
  
        private static void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            var viewModel = args.DragablzItem.DataContext as TabViewModel;
            viewModel.Cleanup();
        }

        public bool HasTabs
        {
            get { return Tabs.Count > 0; }
        }

        public bool HasNoTabs
        {
            get { return !HasTabs; }
        }
    }
}
