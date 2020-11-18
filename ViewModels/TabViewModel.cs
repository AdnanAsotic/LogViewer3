using LogViewer3.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Data;
using System.Windows.Input;

namespace LogViewer3.ViewModels
{
    public class TabViewModel : BindableBase
    {
        Timer _timer = null;
        long _offset = 0;
        int _index = 1;

        private BlockingCollection<LogLinesContainer> _queue = new BlockingCollection<LogLinesContainer>();

        public TabViewModel(IConfigurationService configurationService, bool loadOnStartup)
        {
            ConfigurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
            _loadonStartup = loadOnStartup;

            _timer = new Timer(1000);
            _timer.Elapsed += _timer_Elapsed;

            StartFetchingCommand = new DelegateCommand(() => 
            {
                if (!Started)
                {
                    Started = true;
                    _timer.Start();
                }
            });

            StopFetchingCommand = new DelegateCommand(() => 
            {
                Started = false;
                _timer?.Stop();
            }, () => Started);

            SearchCommand = new DelegateCommand(() => 
            {
                Filter = PreviewFilter;
                _regexFilter = new Regex($"{Filter}", RegexOptions.IgnoreCase);
                Lines.Refresh();
            });
        }

        Regex _regexFilter;

        private void FetchLogLines()
        {
            bool initial = false;

            foreach (var item in _queue.GetConsumingEnumerable())
            {
                PercentageLoaded = item.Percentage;

                foreach (var x in item.Lines)
                {
                    _internalLines.Add(x);
                }

                if (item.Lines.Length > 0)
                {
                    SafeDispatch(() =>
                    {
                        if (item.Percentage == 100)
                        {
                            if (!initial)
                            {
                                Lines = CollectionViewSource.GetDefaultView(_internalLines);
                                Lines.Filter = GlobalLogLinesFilter;
                                initial = true;
                            }
                            else
                            {
                                Lines.Refresh();
                            }
                        }
                    });
                }
            }
        }

        private bool GlobalLogLinesFilter(object e)
        {
            if (string.IsNullOrEmpty(Filter))
            {
                return true;
            }

            return e is LogLine line && _regexFilter.Match(line.Data).Success;
        }

        private bool _started;
        public bool Started
        {
            get { return _started; }
            set { SetProperty(ref _started, value); }
        }

        private string _header = "Neu";
        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }

        private string _filePath = "";
        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    OpenFile(value);
                }

                SetProperty(ref _filePath, value);

                FileInfo info = new FileInfo(value);
                Header = info.Name;
            }
        }

        private string _previewFilter = "";
        public string PreviewFilter
        {
            get => _previewFilter;
            set
            {
                SetProperty(ref _previewFilter, value);
            }
        }

        private string _filter = "";
        public string Filter
        {
            get => _filter;
            set
            {
                SetProperty(ref _filter, value);
            }
        }

        private bool _autoLoad = true;
        public bool AutoLoad
        {
            get => _autoLoad;
            set => SetProperty(ref _autoLoad, value);
        }

        private bool _ignoreCasing = true;
        public bool IgnoreCasing
        {
            get { return _ignoreCasing; }
            set { SetProperty(ref _ignoreCasing, value); }
        }

        private bool _loadonStartup;
        public bool LoadOnStartup
        {
            get { return _loadonStartup; }
            set 
            { 
                SetProperty(ref _loadonStartup, value);

                var loads = ConfigurationService.GetAutoLoads();
                if (_loadonStartup)
                {
                    loads.Add(new AutoLoadConfigItem() { Path = FilePath, Name = new FileInfo(FilePath).Name });
                }
                else
                {
                    loads = loads.Where(l => l.Path != FilePath).ToList();
                }

                ConfigurationService.SaveAutoLoads(loads);
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand StartFetchingCommand { get; }
        public ICommand StopFetchingCommand { get; }

        private void OpenFile(string path)
        {
            _queue.Dispose();
            _queue = new BlockingCollection<LogLinesContainer>();

            _offset = 0;
            _index = 0;

            StopFetchingCommand.Execute(null);

            _internalLines.Clear();

            Task.Run(() => {
                FetchLogLines();
            });

            Task.Run(() =>
            {
                GetLogLines(path);
            });
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Refresh();
        }

        long _byteTotalSize = 0;
        long _byteRemaining = 0;
        long _byteRead = 0;

        private void SafeDispatch(Action action)
        {
            if (App.Current != null && App.Current.Dispatcher != null) App.Current.Dispatcher.Invoke(action);
        }

        private void GetLogLines(string path)
        {
            SafeDispatch(() =>
            {
                IsFileLoaded = false;
            });

            try
            {
                using (var fs = File.OpenRead(path))
                {
                    _byteTotalSize = fs.Length;
                    _byteRemaining = _byteTotalSize - _offset;

                    fs.Seek(_offset, SeekOrigin.Begin);

                    using (var sr = new StreamReader(fs))
                    {
                        string line = null;

                        List<LogLine> tempList = new List<LogLine>();

                        while ((line = sr.ReadLine()) != null)
                        {
                            _byteRead += Encoding.UTF8.GetBytes(line).Length;

                            tempList.Add(new LogLine()
                            {
                                Data = line,
                                LineNumber = _index
                            });

                            _index += 1;

                            if (_index % 10000 == 0)
                            {
                                _queue.Add(new LogLinesContainer() { Lines = tempList.ToArray(), Percentage = (int)(_byteRead * 100 / _byteRemaining) });
                                tempList.Clear();
                            }
                        }

                        _queue.Add(new LogLinesContainer() { Lines = tempList.ToArray(), Percentage = 100 });

                        _offset = fs.Position;
                    }
                }
            }
            catch (Exception)
            {

            }

            SafeDispatch(() =>
            {
                IsFileLoaded = true;
            });
        }

        private int _percentageLoaded;
        public int PercentageLoaded
        {
            get { return _percentageLoaded; }
            set { SetProperty(ref _percentageLoaded, value); }
        }

        private bool _isFileLoaded;
        public bool IsFileLoaded
        {
            get { return _isFileLoaded; }
            set { SetProperty(ref _isFileLoaded, value); }
        }

        public void Refresh()
        {
            if (!IsFileLoaded) return;

            if (string.IsNullOrEmpty(FilePath)) return;

            GetLogLines(FilePath);
        }

        private List<LogLine> _internalLines = new List<LogLine>();

        private ICollectionView _lines;
        public ICollectionView Lines
        {
            get { return _lines; }
            set { SetProperty(ref _lines, value); }
        }

        public IConfigurationService ConfigurationService { get; }

        public void Cleanup()
        {
            _queue.CompleteAdding();
            StopFetchingCommand?.Execute(null);
            _timer.Close();
            _timer = null;

            _internalLines.Clear();
            _internalLines = null;
            _lines = null;

            Started = false;
            AutoLoad = false;
            
            GC.Collect(0);
        }
    }
}
