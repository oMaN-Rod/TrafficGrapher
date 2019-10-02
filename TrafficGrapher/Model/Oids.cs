using System.Collections.Generic;
using System.Linq;
using Lextm.SharpSnmpLib;

namespace TrafficGrapher.Model
{
    public static class Oids
    {
        /// <summary>
        /// Device system up time, fixed value .1.3.6.1.2.1.1.3.0
        /// </summary>
        public static Oid SysUpTime = new Oid("1.3.6.1.2.1.1.3.0");

        /// <summary>
        /// Interface indices, fixed value
        /// </summary>
        public static Oid IfIndex = new Oid("1.3.6.1.2.1.2.2.1.1");

        /// <summary>
        /// Interface description, indexed value .1.3.6.1.2.1.2.2.1.2.{portIdx}
        /// </summary>
        public static Oid IfDescr = new Oid("1.3.6.1.2.1.2.2.1.2");

        /// <summary>
        /// Interface MTU, indexed value .1.3.6.1.2.1.2.2.1.4.{portIdx}
        /// </summary>
        public static Oid IfMtu = new Oid("1.3.6.1.2.1.2.2.1.4");

        /// <summary>
        /// Interface speed, indexed value .1.3.6.1.2.1.2.2.1.5.{portIdx}
        /// </summary>
        public static Oid IfSpeed = new Oid("1.3.6.1.2.1.2.2.1.5");

        /// <summary>
        /// Interface MAC address, indexed value .1.3.6.1.2.1.2.2.1.6.{portIdx}
        /// </summary>
        public static Oid IfPhysAddress = new Oid("1.3.6.1.2.1.2.2.1.6");

        /// <summary>
        /// Interface admin status, indexed value .1.3.6.1.2.1.2.2.1.7.{portIdx}
        /// </summary>
        public static Oid IfAdminStatus = new Oid("1.3.6.1.2.1.2.2.1.7");

        /// <summary>
        /// Interface operation status, indexed value .1.3.6.1.2.1.2.2.1.8.{portIdx}
        /// </summary>
        public static Oid IfOperStatus = new Oid("1.3.6.1.2.1.2.2.1.8");

        /// <summary>
        /// Interface last change, indexed value .1.3.6.1.2.1.2.2.1.9.{portIdx}
        /// </summary>
        public static Oid IfLastChange = new Oid("1.3.6.1.2.1.2.2.1.9");

        /// <summary>
        /// Interface in octets, indexed value .1.3.6.1.2.1.2.2.1.10.{portIdx}
        /// </summary>
        public static Oid IfInOctets = new Oid("1.3.6.1.2.1.2.2.1.10");
        /// <summary>
        /// Interface in hc octets (64 bit counter), indexed value .1.3.6.1.2.1.31.1.1.1.6.{portIdx}
        /// </summary>
        public static Oid IfHcInOctets = new Oid("1.3.6.1.2.1.31.1.1.1.6");

        /// <summary>
        /// Interface in unicast packetst, indexed value .1.3.6.1.2.1.2.2.1.11.{portIdx}
        /// </summary>
        public static Oid IfInUcastPkts = new Oid("1.3.6.1.2.1.2.2.1.11");

        /// <summary>
        /// Interface in discards, indexed value .1.3.6.1.2.1.2.2.1.13.{portIdx}
        /// </summary>
        public static Oid IfInDiscards = new Oid("1.3.6.1.2.1.2.2.1.13");

        /// <summary>
        /// Interface in errors, indexed value .1.3.6.1.2.1.2.2.1.14.{portIdx}
        /// </summary>
        public static Oid IfInErrors = new Oid("1.3.6.1.2.1.2.2.1.14");

        /// <summary>
        /// Interface in unknown protocols, indexed value .1.3.6.1.2.1.2.2.1.15.{portIdx}
        /// </summary>
        public static Oid IfInUnknownProtos = new Oid("1.3.6.1.2.1.2.2.1.15");

        /// <summary>
        /// Interface out octets, indexed value .1.3.6.1.2.1.2.2.1.16.{portIdx}
        /// </summary>
        public static Oid IfOutOctets = new Oid("1.3.6.1.2.1.2.2.1.16");

        /// <summary>
        /// Interface in hc octets (64 bit counter), indexed value .1.3.6.1.2.1.31.1.1.1.10.{portIdx}
        /// </summary>
        public static Oid IfHcOutOctets = new Oid("1.3.6.1.2.1.31.1.1.1.10");

        /// <summary>
        /// Interface out unicast packets, indexed value .1.3.6.1.2.1.2.2.1.17.{portIdx}
        /// </summary>
        public static Oid IfOutUcastPkts = new Oid("1.3.6.1.2.1.2.2.1.17");

        /// <summary>
        /// Interface out discards, indexed value .1.3.6.1.2.1.2.2.1.19.{portIdx}
        /// </summary>
        public static Oid IfOutDiscards = new Oid("1.3.6.1.2.1.2.2.1.19");

        /// <summary>
        /// Interface out errors, indexed value .1.3.6.1.2.1.2.2.1.20.{portIdx}
        /// </summary>
        public static Oid IfOutErrors = new Oid("1.3.6.1.2.1.2.2.1.20");

        /// <summary>
        /// Interface name, indexed value .1.3.6.1.2.1.31.1.1.1.1.{portIdx}
        /// </summary>
        public static Oid IfName = new Oid("1.3.6.1.2.1.31.1.1.1.1");

        /// <summary>
        /// Interface name, indexed value .1.3.6.1.2.1.31.1.1.1.15.{portIdx}
        /// </summary>
        public static Oid IfHighSpeed = new Oid("1.3.6.1.2.1.31.1.1.1.15");

        /// <summary>
        /// Interface alias, indexed value .1.3.6.1.2.1.31.1.1.1.18.{portIdx}
        /// </summary>
        public static Oid IfAlias = new Oid("1.3.6.1.2.1.31.1.1.1.18");

        /// <summary>
        /// Interface duplex (dot3StatsDuplexStatus), indexed value .1.3.6.1.2.1.10.7.2.1.19
        /// </summary>
        public static Oid IfDuplex = new Oid("1.3.6.1.2.1.10.7.2.1.19");

        /// <summary>
        /// Interface IP addresses interface indexes, indexed values .1.3.6.1.2.1.4.20.1.2.{ipAddress}
        /// </summary>
        /// <returns></returns>
        public static Oid IfIpAddress = new Oid("1.3.6.1.2.1.4.20.1.2");

        public static List<ObjectIdentifier> StandardCounters(int index)
        {
            return new List<ObjectIdentifier>
            {
                IfInOctets.AddIndex(index),
                IfOutOctets.AddIndex(index),
                SysUpTime.RootNode
            };
        }

        public static List<ObjectIdentifier> HighCounters(int index)
        {
            return new List<ObjectIdentifier>
            {
                IfHcInOctets.AddIndex(index),
                IfHcOutOctets.AddIndex(index),
                SysUpTime.RootNode
            };
        }

        public static List<ObjectIdentifier> InterfaceInfo(int index)
        {
            return new List<ObjectIdentifier>
            {
                IfDescr.AddIndex(index),
                IfMtu.AddIndex(index),
                IfSpeed.AddIndex(index),
                IfPhysAddress.AddIndex(index),
                IfAdminStatus.AddIndex(index),
                IfOperStatus.AddIndex(index),
                IfName.AddIndex(index),
                IfHighSpeed.AddIndex(index),
                IfAlias.AddIndex(index),
                IfDuplex.AddIndex(index)
            };
        }

        public static List<ObjectIdentifier> AddIndex(int index)
        {
            return typeof(Oids).GetFields().Select(p => ((Oid) p.GetValue(null)).AddIndex(index)).ToList();
        }
    }
}