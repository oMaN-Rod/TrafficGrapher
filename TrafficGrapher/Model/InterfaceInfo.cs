using System.Net;
using TrafficGrapher.Model.Enums;

namespace TrafficGrapher.Model
{
    public class InterfaceInfo
    {
        public int Index { get; set; }
        public string Description { get; set; }
        public int Mtu { get; set; }
        public string Speed { get; set; }
        public string PhyAddress { get; set; }
        public IPAddress IpAddress { get; set; }
        public Status AdminStatus { get; set; }
        public Status OperStatus { get; set; }
        public Duplex Duplex { get; set; }
        public string Name { get; set; }
        public string HighSpeed { get; set; }
        public string Alias { get; set; }
        public string Status => $"{AdminStatus} ({HighSpeed}/{Duplex})";
    }
}