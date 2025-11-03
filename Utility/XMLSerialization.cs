using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Utility
{
    public class SerializeDeserialize<T>
    {
        StringBuilder sbData;
        StringWriter swWriter;
        XmlDocument xDoc;
        XmlNodeReader xNodeReader;
        XmlSerializer xmlSerializer;
        public SerializeDeserialize()
        {
            sbData = new StringBuilder();
        }
        public string SerializeData(T data)
        {
            XmlSerializer Serial = new XmlSerializer(typeof(T));
            swWriter = new StringWriter(sbData);
            Serial.Serialize(swWriter, data);
            return sbData.ToString();
        }
        public T DeserializeData(string dataXML)
        {
            xDoc = new XmlDocument();
            xDoc.LoadXml(dataXML);
            xNodeReader = new XmlNodeReader(xDoc.DocumentElement);
            xmlSerializer = new XmlSerializer(typeof(T));
            var Data = xmlSerializer.Deserialize(xNodeReader);
            T DeserializedData = (T)Data;
            return DeserializedData;
        }
    }
}
