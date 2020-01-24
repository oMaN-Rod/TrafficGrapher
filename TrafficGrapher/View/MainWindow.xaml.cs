using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using TrafficGrapher.Model;
using TrafficGrapher.Model.Enums;
using TrafficGrapher.Model.Messages;
using TrafficGrapher.ViewModel;

namespace TrafficGrapher.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string,string> _args;
        public MainWindow()
        {
            InitializeComponent();
            _args = ParseCommandLineArgs();

            Messenger.Default.Register<DarkModeEnabledMessage>(this, SetLightDark);
        }

        private Dictionary<string, string> ParseCommandLineArgs()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            var args = new Dictionary<string, string>();

            for (var idx = 1; idx < commandLineArgs.Length; idx += 2)
            {
                var arg = commandLineArgs[idx].Replace("/", "");
                args.Add(arg, commandLineArgs[idx + 1]);
            }

            return args;
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            ((MainViewModel) DataContext).SaveSettings();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            SetLightDark(new DarkModeEnabledMessage(Properties.Settings.Default.DarkModeEnabled));
            if (_args.Count >= 1)
            {
                var vm = (MainViewModel) DataContext;
                foreach (KeyValuePair<string,string> item in _args)
                {
                    switch (item.Key)
                    {
                        case "i":
                        case "ip":
                            vm.GraphSettings.IpAddress = item.Value;
                            break;
                        case "s":
                        case "snmpcommunity":
                            vm.GraphSettings.SnmpCommunity = item.Value;
                            break;
                        case "p":
                        case "portindex":
                            if (int.TryParse(item.Value, out int index))
                            {
                                vm.GraphSettings.InterfaceIndex = index;
                            }
                            break;
                        case "c":
                        case "countertype":
                            vm.GraphSettings.CounterType = item.Value == "32" ? CounterType.Counter32 : CounterType.Counter64;
                            break;
                        case "pi":
                        case "pollinterval":
                            if (int.TryParse(item.Value, out int interval))
                            {
                                vm.GraphSettings.PollInterval = interval;
                            }
                            break;
                        case "ts":
                        case "timespan":
                            if (int.TryParse(item.Value, out int timespan))
                            {
                                vm.GraphSettings.DefaultTimeSpan = timespan;
                            }
                            break;
                        case "g":
                        case "graphunits":
                            vm.GraphSettings.CounterUnit = item.Value == "bytes" ? CounterUnit.Bytes : CounterUnit.Bits;
                            break;
                        case "pf":
                        case "prefix":
                            if (Enum.TryParse(item.Value, out CounterPrefix prefix))
                            {
                                vm.GraphSettings.CounterPrefix = prefix;
                            }
                            break;
                    }
                }
                vm.StartCommand.Execute(null);
            }
        }

        public void SetLightDark(DarkModeEnabledMessage message)
        {
            var existingResourceDictionary = Application.Current.Resources.MergedDictionaries
                .Where(rd => rd.Source != null)
                .SingleOrDefault(rd => Regex.Match(rd.Source.OriginalString, @"(\/MaterialDesignThemes.Wpf;component\/Themes\/MaterialDesignTheme\.)((Light)|(Dark))").Success);
            if (existingResourceDictionary == null)
                throw new ApplicationException("Unable to find Light/Dark base theme in Application resources.");

            var source =
                $"pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.{(message.DarkModeEnabled ? "Dark" : "Light")}.xaml";
            var newResourceDictionary = new ResourceDictionary() { Source = new Uri(source) };

            Application.Current.Resources.MergedDictionaries.Remove(existingResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(newResourceDictionary);

            var existingMahAppsResourceDictionary = Application.Current.Resources.MergedDictionaries
                .Where(rd => rd.Source != null)
                .SingleOrDefault(rd => Regex.Match(rd.Source.OriginalString, @"(\/MahApps.Metro;component\/Styles\/Accents\/)((BaseLight)|(BaseDark))").Success);
            if (existingMahAppsResourceDictionary == null) return;

            source =
                $"pack://application:,,,/MahApps.Metro;component/Styles/Accents/{(message.DarkModeEnabled ? "BaseDark" : "BaseLight")}.xaml";
            var newMahAppsResourceDictionary = new ResourceDictionary { Source = new Uri(source) };

            Application.Current.Resources.MergedDictionaries.Remove(existingMahAppsResourceDictionary);
            Application.Current.Resources.MergedDictionaries.Add(newMahAppsResourceDictionary);
            ((MainViewModel)DataContext).WindowTheme = Properties.Settings.Default.DarkModeEnabled ? BaseTheme.Dark : BaseTheme.Light;
        }
    }
}
