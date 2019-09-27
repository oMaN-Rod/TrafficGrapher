using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using MaterialDesignThemes.Wpf;
using TrafficGrapher.Model;
using TrafficGrapher.Model.Messages;

namespace TrafficGrapher.ViewModel
{
    public class DialogService : IDialogService
    {
        private const string DialogIdentifier = "RootDialog";
        public async Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            var errorVm = new ErrorModalViewModel()
            {
                Title = title,
                Message = message
            };
            await DialogHost.Show(errorVm, DialogIdentifier);
        }

        public async Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
            var errorVm = new ErrorModalViewModel()
            {
                Title = title,
                Message = error.ToString()
            };
            await DialogHost.Show(errorVm, DialogIdentifier);
        }

        public Task ShowMessage(string message, string title)
        {
            throw new NotImplementedException();
        }

        public Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText,
            Action<bool> afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public async Task<object> ShowDialog<T>(object obj, DialogClosingEventHandler closingEventHandler)
        {
            CloseDialog();
            return await DialogHost.Show((T)obj, DialogIdentifier, closingEventHandler);
        }

        public async Task<object> ShowDialog<T>(object obj)
        {
            CloseDialog();
            return await DialogHost.Show((T)obj, DialogIdentifier);
        }

        public async Task ShowMessageBox(string message, string title)
        {
            await ShowMessage(message, title);
        }

        private void CloseDialog()
        {
            Messenger.Default.Send(new CloseDialogMessage(true));
        }
    }
}
