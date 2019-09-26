using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveCharts.Events;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using TrafficGrapher.Model;
using TrafficGrapher.View;

namespace TrafficGrapher.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private const string DialogIdentifier = "GraphRootDialog";

        private GraphSettings _graphSettings;
        private Graph _graph;
        private bool _dialogOpen;
        
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

        
        public MainViewModel()
        {
            GraphSettings.SettingsChanged += HostSettingsChanged;
        }

        public RelayCommand OpenSettingsCommand => new RelayCommand(OpenSettings);
        public RelayCommand<RangeChangedEventArgs> RangeChangedCommand => new RelayCommand<RangeChangedEventArgs>(RangeChanged);
        public RelayCommand<FrameworkElement> SavePngCommand => new RelayCommand<FrameworkElement>(SaveToPng);
        public RelayCommand StartCommand => new RelayCommand(Start);
        public RelayCommand StopCommand => new RelayCommand(Stop);
        public RelayCommand ClearCommand => new RelayCommand(Clear);
        public RelayCommand SaveSettingsCommand => new RelayCommand(SaveSettings);
        public RelayCommand LoadSettingsCommand => new RelayCommand(LoadSettings);
        public RelayCommand ExportSettingsCommand => new RelayCommand(ExportSettings);
        public RelayCommand SaveToCsvCommand => new RelayCommand(SaveToCsv);

        public RelayCommand FindIndexCommand => new RelayCommand(FindIndex);

        private void HostSettingsChanged(object source, bool changed)
        {
            if (changed) Graph?.Stop();
        }
        private async void OpenSettings()
        {
            CheckDialogOpen();
            await DialogHost.Show(new GraphSettingsModal { DataContext = this }, DialogIdentifier);
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

        private async void SaveToPng(FrameworkElement visual)
        {
            var encoder = new PngBitmapEncoder();
            var fd = new SaveFileDialog()
            {
                Filter = "PNG files (*.png)|*.png",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                FileName = $"TrafficGraph_{DateTime.Now:yyyy-MM-dd hh-mm-ss}.png"
            };
            if (!fd.ShowDialog() == true) return;

            EncodeVisual(visual, fd.FileName, encoder);
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
            Graph = new Graph(GraphSettings);
            Graph.Start();
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
            Properties.Settings.Default.Save();
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
            }
            catch
            {
                CheckDialogOpen();
                var errorVm = new ErrorModalViewModel()
                {
                    Title = "Error loading Graph Settings!",
                    Message = "Default settings loaded."
                };
                await DialogHost.Show(errorVm, DialogIdentifier);
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
        }

        private async void SaveToCsv()
        {
            var fd = new SaveFileDialog()
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = $"TrafficGraph_{DateTime.Now:yyyy-mm-dd h-mm-ss}.csv"
            };
            if (!fd.ShowDialog() == true) return;

            await Task.Factory.StartNew((() =>
            {
                Graph.ToCsv(fd.FileName);
            }));
        }

        private void CheckDialogOpen()
        {
            if (DialogOpen) DialogOpen = false;
        }

        private async void FindIndex()
        {
            CheckDialogOpen();
            var res = await DialogHost.Show(new InterfaceListModal(){DataContext = new InterfaceListViewModel(GraphSettings)}, DialogIdentifier);
            if (res is InterfaceInfo interfaceInfo)
            {
                GraphSettings.InterfaceIndex = interfaceInfo.Index;
            }
            OpenSettings();
        }
    }
}