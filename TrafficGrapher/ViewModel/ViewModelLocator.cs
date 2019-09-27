using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using CommonServiceLocator;
using System;
using MaterialDesignThemes.Wpf;

namespace TrafficGrapher.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<ISnackbarMessageQueue>(() => new SnackbarMessageQueue());
        }

        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>(Guid.NewGuid().ToString());

        public static void Cleanup()
        {
        }
    }
}