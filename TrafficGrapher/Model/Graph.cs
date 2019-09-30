using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Lextm.SharpSnmpLib;
using LiveCharts.Defaults;
using LiveCharts.Geared;

namespace TrafficGrapher.Model
{
    public class Graph : ViewModelBase
    {
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancellationToken;
        private readonly GraphSettings _graphSettings;

        private GearedValues<DateTimePoint> _in, _out;
        private double _from, _to, _count;
        private string _units, _inLegend, _outLegend;
        private PollState _pollState;

        public Graph(GraphSettings graphSettings)
        {
            _graphSettings = graphSettings;
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
        public string Units
        {
            get => _units;
            set { Set(() => Units, ref _units, value); }
        }

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
            Units = _graphSettings.CounterUnit == CounterUnit.Bits ? "bps" : "Bps";
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

        public void Stop()
        {
            if (PollState == PollState.Stopped) return;
            _tokenSource.Cancel();
            _tokenSource?.Dispose();
            PollState = PollState.Stopped;
        }

        private double CalculateDifference(double previousPoll, double currentPoll, double elapsedTime)
        {
            double octets;

            if (previousPoll > currentPoll)
            {
                if (previousPoll < Math.Pow(2, 32))
                {
                    octets = Math.Pow(2, 32) - previousPoll + currentPoll;
                }
                else if (previousPoll < Math.Pow(2, 64))
                {
                    octets = Math.Pow(2, 64) - previousPoll + currentPoll;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                octets = currentPoll - previousPoll;
            }

            if (_graphSettings.CounterUnit == CounterUnit.Bits) return octets * 8 / elapsedTime / 1000;
            return octets / elapsedTime / 1000;
        }

        public void Start()
        {
            if (PollState != PollState.Stopped) return;
            Clear();
            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;

            PollState = PollState.Idle;
            var snmp = new Snmp(_graphSettings.IpAddress, _graphSettings.SnmpCommunity);
            Oids.BuildInterfaceOids(_graphSettings.InterfaceIndex);

            Task.Factory.StartNew(() =>
            {
                _cancellationToken.ThrowIfCancellationRequested();
                var res = snmp.Get(new List<ObjectIdentifier> {Oids.IfName});
                _graphSettings.InterfaceName = res[0].Data.ToString();

                var previousUpTime = new TimeSpan();
                double previousIfIn = 0, previousIfOut = 0;
                double inDifference = 0, outDifference = 0;
                var firstRun = true;
                var startTime = DateTime.Now;

                while (PollState != PollState.Stopped)
                {
                    if (_cancellationToken.IsCancellationRequested) return;
                    PollState = PollState.Polling;
                    if (_graphSettings.CounterType == CounterType.Counter64)
                    {
                        res = snmp.Get(new List<ObjectIdentifier>
                            {Oids.IfHcInOctets, Oids.IfHcOutOctets, Oids.SysUpTime});
                    }
                    else
                    {
                        res = snmp.Get(new List<ObjectIdentifier>
                            {Oids.IfInOctets, Oids.IfOutOctets, Oids.SysUpTime});
                    }

                    if (res != null)
                    {
                        var ifIn = Convert.ToDouble(res[0].Data.ToString());
                        var ifOut = Convert.ToDouble(res[1].Data.ToString());
                        var upTime = TimeSpan.Parse(res[2].Data.ToString());
                        var pollTime = DateTime.Now;
                        if (pollTime >= startTime.AddSeconds(_graphSettings.DefaultTimeSpan))
                        {
                            From = pollTime.Subtract(TimeSpan.FromSeconds(_graphSettings.DefaultTimeSpan)).Ticks;
                            To = pollTime.Ticks;
                        }
                        else
                        {
                            From = startTime.Ticks;
                            To = pollTime.Ticks;
                        }

                        var elapsedTime = upTime.TotalMilliseconds - previousUpTime.TotalMilliseconds;
                        if (Math.Abs(elapsedTime) > 0 && !firstRun)
                        {
                            inDifference = CalculateDifference(previousIfIn, ifIn, elapsedTime);
                            outDifference = CalculateDifference(previousIfOut, ifOut, elapsedTime);
                            In.Add(new DateTimePoint(pollTime, inDifference));
                            Out.Add(new DateTimePoint(pollTime, outDifference));
                        }
                        else if (firstRun)
                        {
                            firstRun = false;
                        }

                        previousUpTime = upTime;
                        previousIfIn = ifIn;
                        previousIfOut = ifOut;
                    }

                    Count = In.Count;
                    InLegend = LegendText(inDifference);
                    OutLegend = LegendText(outDifference);
                    PollState = PollState.Idle;
                    Thread.Sleep(_graphSettings.PollInterval);
                }
            }, _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
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