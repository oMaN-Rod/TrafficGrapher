using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using TrafficGrapher.Model;
using TrafficGrapher.Model.Messages;
using TrafficGrapher.ViewModel;

namespace TrafficGrapher.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<DarkModeEnabledMessage>(this, SetLightDark);
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            ((MainViewModel) DataContext).SaveSettings();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            SetLightDark(new DarkModeEnabledMessage(Properties.Settings.Default.DarkModeEnabled));
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
