namespace TrafficGrapher.Model
{
    public class InterfaceInfo
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }

        public InterfaceInfo(int index, string name, string alias)
        {
            Index = index;
            Name = name;
            Alias = alias;
        }
    }
}