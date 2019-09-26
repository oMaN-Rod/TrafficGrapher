using System.IO;
using System.Xml.Serialization;

namespace TrafficGrapher.Model
{
    public static class SettingsManager
    {
        public static void Write(object obj, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                new XmlSerializer(obj.GetType()).Serialize(writer, obj);
                writer.Close();
            }
        }

        public static T Read<T>(string filePath)
        {
            using (TextReader reader = new StringReader(File.ReadAllText(filePath)))
            {
                return (T) new XmlSerializer(typeof(T)).Deserialize(reader);
            }
        }
    }
}