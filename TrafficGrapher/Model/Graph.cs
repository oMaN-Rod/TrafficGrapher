using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using TrafficGrapher.Model.Enums;
using TimeoutException = Lextm.SharpSnmpLib.Messaging.TimeoutException;

namespace TrafficGrapher.Model
{
    public class Graph : ViewModelBase
    {
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancellationToken;
        private GearedValues<DateTimePoint> _in, _out;
        private double _from, _to, _count;
        private string _inLegend, _outLegend;
        private PollState _pollState;
        private readonly IDialogService _dialogService;
        private readonly GraphSettings _graphSettings;

        public Graph(IDialogService dialogService, GraphSettings graphSettings)
        {
            _graphSettings = graphSettings;
            _dialogService = dialogService;
            _pollState = PollState.Stopped;
        }

        public GearedValues<DateTimePoint> In
        {
            get => _in ?? (_in = new GearedValues<DateTimePoint>().WithQuality(Quality.Highest));
            set { Set(() => In, ref _in, value); }
        }
        public GearedValues<DateTimePoint> Out
        {
            get => _out ?? (_out = new GearedValues<DateTimePoint>().WithQuality(Quality.Highest));
            set { Set(() => Out, ref _out, value); }
        }
        public double From
        {
            get => _from;
            set { Set(() => From, ref _from, value); }
        }
        public double To
        {
            get
            {
                if (Math.Abs(_to) <= 0) _to = _from + 1;
                return _to;
            }
            set { Set(() => To, ref _to, value); }
        }
        public double Count
        {
            get => _count;
            set { Set(() => Count, ref _count, value); }
        }
        public string Units => _graphSettings.CounterUnit == CounterUnit.Bits ? "bps" : "Bps";

        public string InMinLegend => In.Count >= 1 ? LegendText(In.Select(p => p.Value).ToArray().Min()) : "";

        public string InMaxLegend => In.Count >= 1 ? LegendText(In.Select(p => p.Value).ToArray().Max()) : "";

        public string InAvgLegend => In.Count >= 1 ? LegendText(In.Select(p => p.Value).ToArray().Average()) : "";

        public string OutMinLegend => Out.Count >= 1 ? LegendText(Out.Select(p => p.Value).ToArray().Min()) : "";

        public string OutMaxLegend => Out.Count >= 1 ? LegendText(Out.Select(p => p.Value).ToArray().Max()) : "";

        public string OutAvgLegend => Out.Count >= 1 ? LegendText(Out.Select(p => p.Value).ToArray().Average()) : "";

        public string InLegend
        {
            get => _inLegend;
            set
            {
                Set(() => InLegend, ref _inLegend, value);
                RaisePropertyChanged(nameof(InMinLegend));
                RaisePropertyChanged(nameof(InMaxLegend));
                RaisePropertyChanged(nameof(InAvgLegend));
            }
        }

        public string OutLegend
        {
            get => _outLegend;
            set
            {
                Set(() => OutLegend, ref _outLegend, value);
                RaisePropertyChanged(nameof(OutMinLegend));
                RaisePropertyChanged(nameof(OutMaxLegend));
                RaisePropertyChanged(nameof(OutAvgLegend));
            }
        }

        public PollState PollState
        {
            get => _pollState;
            set { Set(() => PollState, ref _pollState, value); }
        }

        private string LegendText(double difference)
        {
            RaisePropertyChanged(nameof(Units));
            switch (_graphSettings.CounterPrefix)
            {
                case CounterPrefix.Kilo:
                    return $"{Math.Round(difference * 1000, 2)} K{Units}";
                case CounterPrefix.Mega:
                    return $"{Math.Round(difference, 2)} M{Units}";
                case CounterPrefix.Giga:
                    return $"{Math.Round(difference / 1000, 2)} G{Units}";
                default:
                    string res;
                    if (difference >= 1000)
                    {
                        res = $"{Math.Round(difference / 1000, 2)} G{Units}";
                    }
                    else if (difference >= 1 && difference < 1000)
                    {
                        res = $"{Math.Round(difference, 2)} M{Units}";
                    }
                    else
                    {
                        res = $"{Math.Round(difference * 1000, 2)} K{Units}";
                    }
                    return res;
            }
        }

        public void Pause()
        {
            if (PollState == PollState.Stopped) return;
            PollState = PollState.Paused;
        }

        public void Stop()
        {
            if (PollState == PollState.Stopped) return;
            _tokenSource.Cancel();
            _tokenSource?.Dispose();
            PollState = PollState.Stopped;
        }

        public void Start()
        {
            if (PollState == PollState.Paused)
            {
                PollState = PollState.Idle;
                return;
            }

            if (PollState != PollState.Stopped) return;
            Clear();
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;

            Task.Factory.StartNew(Poller, _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                .ContinueWith(OnPollError, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void Poller()
        {
            _cancellationToken.ThrowIfCancellationRequested();
            var snmp = new Snmp(_graphSettings.IpAddress, _graphSettings.SnmpCommunity);
            _graphSettings.InterfaceName = snmp.Get(Oids.IfName.AddIndex(_graphSettings.InterfaceIndex)).Data.ToString();

            var now = DateTime.Now;
            var inData = new PollData(now);
            var outData = new PollData(now);
            var firstRun = true;

            PollState = PollState.Idle;
            while (PollState != PollState.Stopped)
            {
                if (_cancellationToken.IsCancellationRequested) return;
                if (PollState == PollState.Paused)
                {
                    Thread.Sleep(_graphSettings.PollInterval);
                    continue;
                }

                PollState = PollState.Polling;
                var res = snmp.Get(_graphSettings.CounterType == CounterType.Counter64
                    ? Oids.StandardCounters(_graphSettings.InterfaceIndex)
                    : Oids.HighCounters(_graphSettings.InterfaceIndex));

                if (res != null)
                {
                    inData.Current = Convert.ToDouble(res[0].Data.ToString());
                    outData.Current = Convert.ToDouble(res[1].Data.ToString());
                    inData.UpTime = outData.UpTime = TimeSpan.Parse(res[2].Data.ToString());
                    inData.PollTime = outData.PollTime = DateTime.Now;
                    if (inData.PollTime >= inData.StartTime.AddSeconds(_graphSettings.DefaultTimeSpan))
                    {
                        From = inData.PollTime.Subtract(TimeSpan.FromSeconds(_graphSettings.DefaultTimeSpan)).Ticks;
                        To = inData.PollTime.Ticks;
                    }
                    else
                    {
                        From = inData.StartTime.Ticks;
                        To = inData.PollTime.Ticks;
                    }

                    if (Math.Abs(inData.ElapsedTime) > 0 && !firstRun)
                    {
                        In.Add(new DateTimePoint(inData.PollTime, inData.CalcDiff(_graphSettings.CounterUnit)));
                        Out.Add(new DateTimePoint(outData.PollTime, outData.CalcDiff(_graphSettings.CounterUnit)));
                    }
                    else if (firstRun)
                    {
                        firstRun = false;
                    }

                    inData.Next();
                    outData.Next();
                }

                Count = In.Count;
                InLegend = LegendText(inData.Diff);
                OutLegend = LegendText(outData.Diff);
                PollState = PollState.Idle;
                Thread.Sleep(_graphSettings.PollInterval);
            }
        }

        private void OnPollError(Task t)
        {
            if (t.Exception == null) return;
            Application.Current.Dispatcher?.Invoke(async () =>
            {
                await _dialogService.ShowError($"{(t.Exception.InnerException?.GetType() == typeof(TimeoutException) ? "A timeout" : "An error")} occurred while polling the device, please check your connectivity and configurations and try again", "Error!", string.Empty, () => { });
            });
        }

        public void Clear()
        {
            In.Clear();
            Out.Clear();
        }
        
        public void ToCsv(string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"DateTime,In (M{Units}),Out (M{Units})");
            foreach (var point in In)
            {
                sb.AppendLine($"{point.DateTime},{point.Value},{Out.FirstOrDefault(p => p.DateTime == point.DateTime)?.Value}");
            }

            using (var writer = new StreamWriter(path))
            {
                writer.Write(sb);
            }
        }
    }
}