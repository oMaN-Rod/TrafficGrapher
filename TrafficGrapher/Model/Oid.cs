using Lextm.SharpSnmpLib;

namespace TrafficGrapher.Model
{
    public class Oid
    {
        public ObjectIdentifier Id { get; set; }
        public string TextualId { get; }
        public ObjectIdentifier RootNode { get; }
        public Oid(string dotted)
        {
            TextualId = dotted;
            RootNode = new ObjectIdentifier(dotted);
        }

        public ObjectIdentifier AddIndex(int index)
        {
            Id = new ObjectIdentifier($"{TextualId}.{index}");
            return Id;
        }
    }
}
