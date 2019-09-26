using System.ComponentModel;
using TrafficGrapher.Converters;

namespace TrafficGrapher.Model
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum CounterType
    {
        [Description("32bit")]
        Counter32,
        [Description("64bit")]
        Counter64
    }
}
