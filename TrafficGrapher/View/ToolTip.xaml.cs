using System.ComponentModel;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using TrafficGrapher.Model;

namespace TrafficGrapher.View
{
    /// <summary>
    /// Interaction logic for ToolTip.xaml
    /// </summary>
    public partial class ToolTip : IChartTooltip
    {
        public ToolTip()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private TooltipData _data;
        public TooltipData Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged(nameof(Data));
            }
        }
        public TooltipSelectionMode? SelectionMode { get; set; }

        private string _units;
        public string Units
        {
            get => _units;
            set
            {
                if (!Equals(_units, value)) _units = value;
                OnPropertyChanged(nameof(Units));
            }
        }

        private Graph _graph;
        public Graph Graph
        {
            get => _graph;
            set
            {
                if (!Equals(_graph, value)) _graph = value;
                OnPropertyChanged(nameof(Graph));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ToolTip_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = (ToolTip) DataContext;
            vm.Units = Graph.Units;
        }
    }
}
