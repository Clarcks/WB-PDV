using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace NFe.AppTeste
{
        public enum SAT
        {
            [XmlEnum("1")]
            Bematech = 1,
            [XmlEnum("2")]
            Tanca = 2,
            [XmlEnum("3")]
            Dimep = 3
        
    }
}
