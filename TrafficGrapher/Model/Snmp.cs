using System.Collections.Generic;
using System.Linq;
using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace TrafficGrapher.Model
{
    public class Snmp
    {
        private readonly string _community;
        private readonly string _host;

        public Snmp(string host = null, string community = null)
        {
            _community = community;
            _host = host;
        }

        public IList<Variable> Get(List<ObjectIdentifier> oids)
        {
            var result = Messenger.Get(VersionCode.V2,
                new IPEndPoint(IPAddress.Parse(_host), 161),
                new OctetString(_community),
                oids.Select(oid => new Variable(oid)).ToList(),
                3000);
            return result;
        }

        public Variable Get(ObjectIdentifier oid)
        {
            var result = Messenger.Get(VersionCode.V2,
                new IPEndPoint(IPAddress.Parse(_host), 161),
                new OctetString(_community),
                new List<Variable> {new Variable(oid)},
                5000);
            return result[0];
        }

        public List<Variable> Walk(ObjectIdentifier oid)
        {
            var result = new List<Variable>();
            Messenger.Walk(VersionCode.V2,
                new IPEndPoint(IPAddress.Parse(_host), 161),
                new OctetString(_community),
                oid,
                result,
                5000,
                WalkMode.WithinSubtree);
            return result;
        }
    }
}