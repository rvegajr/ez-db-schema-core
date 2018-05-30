using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EzDbSchema.Core.Interfaces
{
    public interface IXmlRenderable
    {
        string AsXml();
        XmlNode AsXml(XmlDocument doc);
        void FromXml(string Xml);
        XmlNode FromXml(XmlNode node);
    }
}
