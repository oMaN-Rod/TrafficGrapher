using System;
using System.ComponentModel;
using System.Windows;
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
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            ((MainViewModel) DataContext).SaveSettings();
        }
    }
}
