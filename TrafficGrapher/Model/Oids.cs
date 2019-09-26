using Lextm.SharpSnmpLib;

namespace TrafficGrapher.Model
{
    public static class Oids
    {
        /// <summary>
        /// Device system up time, fixed value .1.3.6.1.2.1.1.3.0
        /// </summary>
        public static ObjectIdentifier SysUpTime = new ObjectIdentifier(".1.3.6.1.2.1.1.3.0");

        /// <summary>
        /// Interface description, dynamic value .1.3.6.1.2.1.2.2.1.2.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfDescr;

        /// <summary>
        /// Interface MTU, dynamic value .1.3.6.1.2.1.2.2.1.4.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfMtu;

        /// <summary>
        /// Interface speed, dynamic value .1.3.6.1.2.1.2.2.1.5.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfSpeed;

        /// <summary>
        /// Interface MAC address, dynamic value .1.3.6.1.2.1.2.2.1.6.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfPhysAddress;

        /// <summary>
        /// Interface admin status, dynamic value .1.3.6.1.2.1.2.2.1.7.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfAdminStatus;

        /// <summary>
        /// Interface operation status, dynamic value .1.3.6.1.2.1.2.2.1.8.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfOperStatus;

        /// <summary>
        /// Interface last change, dynamic value .1.3.6.1.2.1.2.2.1.9.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfLastChange;

        /// <summary>
        /// Interface in octets, dynamic value .1.3.6.1.2.1.2.2.1.10.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfInOctets;

        /// <summary>
        /// Interface in hc octets (64 bit counter), dynamic value .1.3.6.1.2.1.31.1.1.1.6.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfHcInOctets;

        /// <summary>
        /// Interface in unicast packetst, dynamic value .1.3.6.1.2.1.2.2.1.11.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfInUcastPkts;

        /// <summary>
        /// Interface in discards, dynamic value .1.3.6.1.2.1.2.2.1.13.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfInDiscards;

        /// <summary>
        /// Interface in errors, dynamic value .1.3.6.1.2.1.2.2.1.14.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfInErrors;

        /// <summary>
        /// Interface in unknown protocols, dynamic value .1.3.6.1.2.1.2.2.1.15.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfInUnknownProtos;

        /// <summary>
        /// Interface out octets, dynamic value .1.3.6.1.2.1.2.2.1.16.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfOutOctets;

        /// <summary>
        /// Interface in hc octets (64 bit counter), dynamic value .1.3.6.1.2.1.31.1.1.1.10.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfHcOutOctets;

        /// <summary>
        /// Interface out unicast packets, dynamic value .1.3.6.1.2.1.2.2.1.17.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfOutUcastPkts;

        /// <summary>
        /// Interface out discards, dynamic value .1.3.6.1.2.1.2.2.1.19.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfOutDiscards;

        /// <summary>
        /// Interface out errors, dynamic value .1.3.6.1.2.1.2.2.1.20.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfOutErrors;

        /// <summary>
        /// Interface name, dynamic value .1.3.6.1.2.1.31.1.1.1.1.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfName;

        /// <summary>
        /// Interface name, dynamic value .1.3.6.1.2.1.31.1.1.1.15.{portIdx}
        /// </summary>
        public static ObjectIdentifier IfHighSpeed;

        public static ObjectIdentifier IfAlias;

        /// <summary>
        /// Builds the interface OIDs.
        /// </summary>
        /// <param name="portIdx">Index of the port.</param>
        public static void BuildInterfaceOids(int portIdx)
        {
            IfDescr = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.2.{portIdx}");
            IfMtu = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.4.{portIdx}");
            IfSpeed = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.5.{portIdx}");
            IfPhysAddress = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.6.{portIdx}");
            IfAdminStatus = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.7.{portIdx}");
            IfOperStatus = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.8.{portIdx}");
            IfLastChange = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.9.{portIdx}");
            IfInOctets = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.10.{portIdx}");
            IfHcInOctets = new ObjectIdentifier($".1.3.6.1.2.1.31.1.1.1.6.{portIdx}");
            IfInUcastPkts = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.11.{portIdx}");
            IfInDiscards = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.13.{portIdx}");
            IfInErrors = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.14.{portIdx}");
            IfInUnknownProtos = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.15.{portIdx}");
            IfOutOctets = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.16.{portIdx}");
            IfHcOutOctets = new ObjectIdentifier($".1.3.6.1.2.1.31.1.1.1.10.{portIdx}");
            IfOutUcastPkts = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.17.{portIdx}");
            IfOutDiscards = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.19.{portIdx}");
            IfOutErrors = new ObjectIdentifier($".1.3.6.1.2.1.2.2.1.20.{portIdx}");
            IfName = new ObjectIdentifier($".1.3.6.1.2.1.31.1.1.1.1.{portIdx}");
            IfHighSpeed = new ObjectIdentifier($".1.3.6.1.2.1.31.1.1.1.15.{portIdx}");
            IfAlias = new ObjectIdentifier($".1.3.6.1.2.1.31.1.1.1.18.{portIdx}");
        }
    }
}