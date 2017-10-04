using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NFe.Classes
{
    [Serializable]
    public class infNFeSupl : IXmlSerializable
    {
        /// <summary>
        /// ZX02 - Texto com o QR-Code impresso no DANFE NFC-e
        /// </summary>
        /// 
        public string CDATA { get; set; }
        public string qrCode { get; set; }

         

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(typeof(infNFeSupl).Name);
            reader.ReadStartElement("qrCode");
            qrCode = reader.ReadString();
            reader.ReadEndElement();
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("qrCode");
            writer.WriteCData(qrCode);
            writer.WriteEndElement();
        }
    }
}