using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
using System;

namespace TrafficGrapher.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>(Guid.NewGuid().ToString());

        public static void Cleanup()
        {
        }
    }
}