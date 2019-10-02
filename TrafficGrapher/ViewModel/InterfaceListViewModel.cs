using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using Lextm.SharpSnmpLib;
using TrafficGrapher.Model;
using TrafficGrapher.Model.Enums;
using TimeoutException = Lextm.SharpSnmpLib.Messaging.TimeoutException;

namespace TrafficGrapher.ViewModel
{
    public class InterfaceListViewModel : ViewModelBase
    {
        private ObservableCollection<InterfaceInfo> _interfaces;
        private InterfaceInfo _selectedInterface;
        private bool _listVisible = true;
        private string _errorMsg;
        private bool _isBusy;

        public ObservableCollection<InterfaceInfo> Interfaces
        {
            get => _interfaces ?? (_interfaces = new ObservableCollection<InterfaceInfo>());
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

        public void GetInterfaceList(GraphSettings settings)
        {
            ListVisible = false;
            IsBusy = true;

            Task.Run(() => GetInterfaceInfo(settings)).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    IsBusy = false;
                    ErrorMsg = $"{(t.Exception.InnerException?.GetType() == typeof(TimeoutException) ? "A timeout" : "An error")} occurred while polling the device, please check your connectivity and configurations and try again";
                }
                else
                {
                    ListVisible = true;
                }
            });
        }

        public IPAddress ParseIP(Variable variable)
        {
            if (variable == null) return null;
            return IPAddress.TryParse(variable.Id.ToString().Replace(Oids.IfIpAddress.TextualId, "").TrimEnd('.').TrimStart('.'),
                out IPAddress address) ? address : null;
        }

        private void GetInterfaceInfo(GraphSettings settings)
        {
            var snmp = new Snmp(settings.IpAddress, settings.SnmpCommunity);
            IList<Variable> res;
            res = snmp.Walk(Oids.IfIndex.RootNode);

            foreach (var variable in res)
            {
                if (!int.TryParse(variable.Data.ToString(), out int idx)) continue;
                IList<Variable> interfaces = new List<Variable>();
                IList<Variable> ips = new List<Variable>();
                try
                {
                    interfaces = snmp.Get(Oids.InterfaceInfo(idx));
                    ips = snmp.Walk(Oids.IfIpAddress.RootNode);
                }
                catch
                {
                    continue;
                }

                IPAddress ip = null;
                if (ips.Count > 0)
                {
                    ip = ParseIP(ips.FirstOrDefault(r => r.Data.ToString() == idx.ToString()));
                }

                var interfaceInfo = new InterfaceInfo()
                {
                    Index = idx,
                    Description = interfaces.Single(r => r.Id == Oids.IfDescr.AddIndex(idx)).Data.ToString(),
                    Mtu = int.TryParse(interfaces.Single(r => r.Id == Oids.IfMtu.AddIndex(idx)).ToString(),
                        out int mtu)
                        ? mtu
                        : 0,
                    Speed = interfaces.Single(r => r.Id == Oids.IfSpeed.AddIndex(idx)).Data.ToString(),
                    PhyAddress = string.Join(":",
                        ((OctetString) interfaces.Single(r => r.Id == Oids.IfPhysAddress.AddIndex(idx)).Data)
                        .GetRaw()
                        .ToList()
                        .Select(v => v.ToString("X2", CultureInfo.InvariantCulture))),
                    AdminStatus =
                        int.TryParse(
                            interfaces.Single(r => r.Id == Oids.IfAdminStatus.AddIndex(idx)).Data.ToString(),
                            out int adminRes)
                            ? (Status) Enum.ToObject(typeof(Status), adminRes)
                            : Status.Invalid,
                    OperStatus =
                        int.TryParse(
                            interfaces.Single(r => r.Id == Oids.IfOperStatus.AddIndex(idx)).Data.ToString(),
                            out int operRes)
                            ? (Status) Enum.ToObject(typeof(Status), operRes)
                            : Status.Invalid,
                    Name = interfaces.Single(r => r.Id == Oids.IfName.AddIndex(idx)).Data.ToString(),
                    HighSpeed = interfaces.Single(r => r.Id == Oids.IfHighSpeed.AddIndex(idx)).Data.ToString(),
                    Alias = interfaces.Single(r => r.Id == Oids.IfAlias.AddIndex(idx)).Data.ToString(),
                    Duplex = int.TryParse(
                        interfaces.Single(r => r.Id == Oids.IfDuplex.AddIndex(idx)).Data.ToString(),
                        out int duplexRes)
                        ? (Duplex) Enum.ToObject(typeof(Duplex), duplexRes)
                        : Duplex.Invalid,
                    IpAddress = ip

                };

                Application.Current.Dispatcher?.Invoke(() =>
                {
                    Interfaces.Add(interfaceInfo);
                });
            }
        }
    }
}