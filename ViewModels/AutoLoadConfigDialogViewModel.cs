using LogViewer3.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogViewer3.ViewModels
{
    public class AutoLoadConfigDialogViewModel : BindableBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;

        public AutoLoadConfigDialogViewModel(IConfigurationService configurationService)
        {
            ConfigurationService = configurationService;
            Configurations = new ObservableCollection<AutoLoadConfigItem>(ConfigurationService.GetAutoLoads());
            SaveCommand = new DelegateCommand(() =>
            {
                ConfigurationService.SaveAutoLoads(Configurations.ToList());
            });
        }

        private string _title = "Setup Autoload Paths";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ObservableCollection<AutoLoadConfigItem> Configurations { get; set; }
        public IConfigurationService ConfigurationService { get; }
        public ICommand SaveCommand { get; set; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            // todo execution here
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            // todo initi her
        }
    }
}
