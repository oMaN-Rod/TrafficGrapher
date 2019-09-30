using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Views;
using LiveCharts.Events;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using TrafficGrapher.Model;
using TrafficGrapher.Model.Messages;
using TrafficGrapher.View;

namespace TrafficGrapher.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private GraphSettings _graphSettings;
        private Graph _graph;
        private bool _dialogOpen;
        private BaseTheme _windowTheme;
        private bool _darkModeEnabled;
        private SnackbarMessageQueue _snackbar;
        private DialogService _dialogService;

        public GraphSettings GraphSettings
        {
            get => _graphSettings ?? (_graphSettings = new GraphSettings());
            set { Set(() => GraphSettings, ref _graphSettings, value); }
        }
        public Graph Graph
        {
            get => _graph;
            set { Set(() => Graph, ref _graph, value); }
        }
        public bool DialogOpen
        {
            get => _dialogOpen;
            set { Set(() => DialogOpen, ref _dialogOpen, value); }
        }

        public BaseTheme WindowTheme
        {
            get => _windowTheme;
            set { Set(() => WindowTheme, ref _windowTheme, value); }
        }

        public bool DarkModeEnabled
        {
            get => _darkModeEnabled;
            set { Set(() => DarkModeEnabled, ref _darkModeEnabled, value); }
        }

        public SnackbarMessageQueue Snackbar => _snackbar;

        public RelayCommand OpenSettingsCommand => new RelayCommand(OpenSettings);
        public RelayCommand<RangeChangedEventArgs> RangeChangedCommand => new RelayCommand<RangeChangedEventArgs>(RangeChanged);
        public RelayCommand<FrameworkElement> SavePngCommand => new RelayCommand<FrameworkElement>(SaveToPng);
        public RelayCommand StartCommand => new RelayCommand(Start);
        public RelayCommand StopCommand => new RelayCommand(Stop);
        public RelayCommand PauseCommand => new RelayCommand(Pause);
        public RelayCommand ClearCommand => new RelayCommand(Clear);
        public RelayCommand SaveSettingsCommand => new RelayCommand(SaveSettings);
        public RelayCommand LoadSettingsCommand => new RelayCommand(LoadSettings);
        public RelayCommand ExportSettingsCommand => new RelayCommand(ExportSettings);
        public RelayCommand SaveToCsvCommand => new RelayCommand(SaveToCsv);
        public RelayCommand FindIndexCommand => new RelayCommand(FindIndex);
        public RelayCommand<bool> ToggleDarkModeCommand => new RelayCommand<bool>(ToggleDarkMode);

        public MainViewModel(IDialogService dialogService, ISnackbarMessageQueue snackbar)
        {
            DarkModeEnabled = Properties.Settings.Default.DarkModeEnabled;
            _dialogService = (DialogService) dialogService;
            _snackbar = (SnackbarMessageQueue) snackbar;
            Messenger.Default.Register<SettingsChangedMessage>(this, SettingsChanged);
            Messenger.Default.Register<CloseDialogMessage>(this, CloseDialog);
        }

        private void SettingsChanged(SettingsChangedMessage message)
        {
            if (message.Changed) Graph?.Stop();
        }
        private async void OpenSettings()
        {
            await _dialogService.ShowDialog<GraphSettingsModal>(new GraphSettingsModal {DataContext = this});
        }

        private void RangeChanged(RangeChangedEventArgs args)
        {
            var currentRange = args.Range;
            if (currentRange < TimeSpan.TicksPerDay * 2)
            {
                GraphSettings.Formatter = x => new DateTime((long)x).ToString("t");
                return;
            }

            if (currentRange < TimeSpan.TicksPerDay * 60)
            {
                GraphSettings.Formatter = x => new DateTime((long)x).ToString("dd MMM yy");
                return;
            }

            if (currentRange < TimeSpan.TicksPerDay * 540)
            {
                GraphSettings.Formatter = x => new DateTime((long)x).ToString("MMM yy");
                return;
            }

            GraphSettings.Formatter = x => new DateTime((long)x).ToString("yyyy");
        }

        private void SaveToPng(FrameworkElement visual)
        {
            if (!CheckGraphStarted()) return;
            var encoder = new PngBitmapEncoder();
            var fd = new SaveFileDialog()
            {
                Filter = "PNG files (*.png)|*.png",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                FileName = $"TrafficGraph_{DateTime.Now:yyyy-MM-dd hh-mm-ss}.png"
            };
            if (!fd.ShowDialog() == true) return;

            EncodeVisual(visual, fd.FileName, encoder);
            _snackbar.Enqueue($"Graph image saved to {fd.FileName}.", "Ok", () => { });
        }

        private static void EncodeVisual(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            var bitmap = new RenderTargetBitmap((int)(visual.ActualWidth * 1.25), (int)(visual.ActualHeight * 1.25), 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var stream = File.Create(fileName)) encoder.Save(stream);
        }

        private void Start()
        {
            if(Graph?.PollState != PollState.Paused) Graph = new Graph(GraphSettings);
            Graph.Start();
        }

        private void Pause()
        {
            Graph?.Pause();
        }

        private void Stop()
        {
            Graph?.Stop();
        }

        private void Clear()
        {
            Graph?.Clear();
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.IpAddress = GraphSettings.IpAddress;
            Properties.Settings.Default.SnmpCommunity = GraphSettings.SnmpCommunity;
            Properties.Settings.Default.Index = GraphSettings.InterfaceIndex;
            Properties.Settings.Default.InColor = GraphSettings.InColorName;
            Properties.Settings.Default.OutColor = GraphSettings.OutColorName;
            Properties.Settings.Default.CounterType = GraphSettings.CounterType.ToString();
            Properties.Settings.Default.CounterUnit = GraphSettings.CounterUnit.ToString();
            Properties.Settings.Default.CounterPrefix = GraphSettings.CounterPrefix.ToString();
            Properties.Settings.Default.PollInterval = GraphSettings.PollInterval;
            Properties.Settings.Default.DefaultTimeSpan = GraphSettings.DefaultTimeSpan;
            Properties.Settings.Default.DarkModeEnabled = DarkModeEnabled;
            Properties.Settings.Default.Save();
            _snackbar.Enqueue("Settings saved");
        }

        private async void LoadSettings()
        {
            var fd = new OpenFileDialog()
            {
                Filter = "XML files (*.xml)|*.xml",
            };
            if (!fd.ShowDialog() == true) return;
            try
            {
                GraphSettings = SettingsManager.Read<GraphSettings>(fd.FileName);
                SaveSettings();
                _snackbar.Enqueue($"Loaded {fd.FileName} successfully!", "Ok", () => { });
            }
            catch
            {
                await _dialogService.ShowError("Default settings loaded.", "Error loading Graph Settings!", "Ok", () => { });
                GraphSettings = new GraphSettings();
            }
            OpenSettings();
        }

        private void ExportSettings()
        {
            var fd = new SaveFileDialog()
            {
                Filter = "XML files (*.xml)|*.xml",
                FileName = $"TrafficGraph_{GraphSettings.IpAddress.Replace(".", "-")}.xml"
            };
            if (!fd.ShowDialog() == true) return;
            SettingsManager.Write(GraphSettings, fd.FileName);
            _snackbar.Enqueue($"Exported {fd.FileName} successfully!", "Ok", () => { });
        }

        private async void SaveToCsv()
        {
            if(!CheckGraphStarted()) return;
            var fd = new SaveFileDialog()
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = $"TrafficGraph_{DateTime.Now:yyyy-mm-dd h-mm-ss}.csv"
            };
            if (!fd.ShowDialog() == true) return;

            await Task.Factory.StartNew((() =>
            {
                Graph?.ToCsv(fd.FileName);
            }));
            _snackbar.Enqueue($"Saved {fd.FileName} successfully!", "Ok", () => { });
        }

        private async void FindIndex()
        {
            var res = await _dialogService.ShowDialog<InterfaceListModal>(new InterfaceListModal(){ DataContext = new InterfaceListViewModel(GraphSettings) });
            if (res is InterfaceInfo interfaceInfo)
            {
                GraphSettings.InterfaceIndex = interfaceInfo.Index;
            }
            OpenSettings();
        }

        private void ToggleDarkMode(bool darkMode)
        {
            Messenger.Default.Send(new DarkModeEnabledMessage(darkMode));
        }

        private void CloseDialog(CloseDialogMessage message)
        {
            if (message.Close) DialogOpen = false;
        }

        private bool CheckGraphStarted()
        {
            if (Graph == null)
            {
                _snackbar.Enqueue("Please start graph before exporting results", "Ok", () => { });
                return false;
            }

            return true;
        }
    }
}