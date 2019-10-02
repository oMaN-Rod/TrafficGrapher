using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using TrafficGrapher.Model.Enums;
using TrafficGrapher.Model.Messages;
using TrafficGrapher.Properties;

namespace TrafficGrapher.Model
{
    public class GraphSettings : ViewModelBase
    {
        private List<PropertyInfo> _allColors;
        private PropertyInfo _inColorPropertyInfo;
        private PropertyInfo _outColorPropertyInfo;
        private Func<double, string> _formatter;
        private string _ipAddress;
        private string _snmpCommunity;
        private int _interfaceIndex;
        private string _interfaceName;

        public GraphSettings()
        {
            IpAddress = Settings.Default.IpAddress;
            SnmpCommunity = Settings.Default.SnmpCommunity;
            InterfaceIndex = Settings.Default.Index;
            InColorName = Settings.Default.InColor;
            OutColorName = Settings.Default.OutColor;
            PollInterval = Settings.Default.PollInterval;
            DefaultTimeSpan = Settings.Default.DefaultTimeSpan;
            CounterType = Enum.TryParse(Settings.Default.CounterType, true, out CounterType counterType)
                ? counterType
                : CounterType.Counter64;
            CounterUnit = Enum.TryParse(Settings.Default.CounterUnit, true, out CounterUnit counterUnit)
                ? counterUnit
                : CounterUnit.Bits;
            CounterPrefix = Enum.TryParse(Settings.Default.CounterPrefix, true, out CounterPrefix counterPrefix)
                ? counterPrefix
                : CounterPrefix.Mega;
        }

        [XmlIgnore]
        public List<PropertyInfo> AllColors
        {
            get => _allColors ?? (_allColors = typeof(Colors).GetProperties().ToList());
            set { Set(() => AllColors, ref _allColors, value); }
        }

        [XmlIgnore]
        public PropertyInfo InColorPropertyInfo
        {
            get => _inColorPropertyInfo ??
                   (_inColorPropertyInfo = typeof(Colors).GetProperty(nameof(Colors.LimeGreen)));
            set
            {
                Set(() => InColorPropertyInfo, ref _inColorPropertyInfo, value);
                RefreshColors();
            }
        }

        [XmlIgnore]
        public Color InColor
        {
            get => (Color) ColorConverter.ConvertFromString(InColorPropertyInfo?.Name);
            set
            {
                InColorPropertyInfo = typeof(Colors).GetProperty(value.ToString());
                RefreshColors();
            }
        }

        public string InColorName
        {
            get => InColorPropertyInfo?.Name;
            set => InColorPropertyInfo = typeof(Colors).GetProperty(value);
        }

        public Brush InBrush => new SolidColorBrush(InColor);

        [XmlIgnore]
        public PropertyInfo OutColorPropertyInfo
        {
            get => _outColorPropertyInfo ??
                   (_outColorPropertyInfo = typeof(Colors).GetProperty(nameof(Colors.DarkBlue)));
            set
            {
                Set(() => OutColorPropertyInfo, ref _outColorPropertyInfo, value);
                RefreshColors();
            }
        }

        [XmlIgnore]
        public Color OutColor
        {
            get => (Color) ColorConverter.ConvertFromString(OutColorPropertyInfo.Name);
            set
            {
                OutColorPropertyInfo = typeof(Colors).GetProperty(value.ToString());
                RefreshColors();
            }
        }

        public string OutColorName
        {
            get => OutColorPropertyInfo?.Name;
            set => OutColor = (Color) ColorConverter.ConvertFromString(value);
        }

        public Brush OutBrush => new SolidColorBrush(OutColor);

        [XmlIgnore]
        public Func<double, string> Formatter
        {
            get => _formatter ?? (_formatter = x => new DateTime((long) x).ToString("h:mm:ss"));
            set { Set(() => Formatter, ref _formatter, value); }
        }

        public string IpAddress
        {
            get => _ipAddress ?? (_ipAddress = "192.168.1.1");
            set
            {
                Set(() => IpAddress, ref _ipAddress, value);
                Messenger.Default.Send(new SettingsChangedMessage(true));
            }
        }

        public string SnmpCommunity
        {
            get => _snmpCommunity ?? (_snmpCommunity = "public");
            set
            {
                Set(() => SnmpCommunity, ref _snmpCommunity, value);
                Messenger.Default.Send(new SettingsChangedMessage(true));
            }
        }

        public int InterfaceIndex
        {
            get
            {
                if (_interfaceIndex == 0) _interfaceIndex = 1;
                return _interfaceIndex;
            }
            set
            {
                Set(() => InterfaceIndex, ref _interfaceIndex, value);
                Messenger.Default.Send(new SettingsChangedMessage(true));
            }
        }

        public string InterfaceName
        {
            get => _interfaceName;
            set { Set(() => InterfaceName, ref _interfaceName, value); }
        }

        public CounterType CounterType { get; set; }
        public CounterUnit CounterUnit { get; set; }
        public CounterPrefix CounterPrefix { get; set; }
        public int PollInterval { get; set; } = 1000;
        public int DefaultTimeSpan { get; set; } = 30;

        private void RefreshColors()
        {
            RaisePropertyChanged(nameof(InColorPropertyInfo));
            RaisePropertyChanged(nameof(InColor));
            RaisePropertyChanged(nameof(InBrush));
            RaisePropertyChanged(nameof(OutColorPropertyInfo));
            RaisePropertyChanged(nameof(OutColor));
            RaisePropertyChanged(nameof(OutBrush));
        }
    }
}