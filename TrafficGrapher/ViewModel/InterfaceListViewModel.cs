using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Lextm.SharpSnmpLib;
using MaterialDesignThemes.Wpf;
using TrafficGrapher.Model;
using TrafficGrapher.View;

namespace TrafficGrapher.ViewModel
{
    public class InterfaceListViewModel: ViewModelBase
    {
        private List<InterfaceInfo> _interfaces;
        private InterfaceInfo _selectedInterface;
        private bool _listVisible = true;
        private string _errorMsg;
        private bool _isBusy;

        public List<InterfaceInfo> Interfaces
        {
            get => _interfaces ?? (_interfaces = new List<InterfaceInfo>());
            set { Set(() => Interfaces, ref _interfaces, value); }
        }

        public InterfaceInfo SelectedInterface
        {
            get => _selectedInterface;
            set { Set(() => SelectedInterface, ref _selectedInterface, value); }
        }

        public bool ListVisible
        {
            get => _listVisible;
            set { Set(() => ListVisible, ref _listVisible, value); }
        }

        public string ErrorMsg
        {
            get => _errorMsg;
            set { Set(() => ErrorMsg, ref _errorMsg, value); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { Set(() => IsBusy, ref _isBusy, value); }
        }
        public InterfaceListViewModel(GraphSettings settings)
        {
            BuildList(settings);
        }

        private void BuildList(GraphSettings settings)
        {
            var snmp = new Snmp(settings.IpAddress, settings.SnmpCommunity);
            ListVisible = false;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (var variable in snmp.Walk(new ObjectIdentifier(".1.3.6.1.2.1.31.1.1.1.1")))
                    {
                        var index = variable.Id.ToString().Substring(variable.Id.ToString().LastIndexOf('.') + 1);
                        if (!int.TryParse(index, out int idx)) continue;
                        Oids.BuildInterfaceOids(idx);
                        var alias = snmp.Get(Oids.IfAlias);
                        Interfaces.Add(new InterfaceInfo(idx, variable.Data.ToString(), alias.Data.ToString()));
                    }
                    ListVisible = true;
                }
                catch
                {
                    IsBusy = false;
                    ErrorMsg = "There was an error polling the device interfaces, please check your settings and try again";
                }
            });
            IsBusy = true;
        }
    }
}
