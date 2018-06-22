using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EzDbSchema.Core.Interfaces
{
    public interface IXmlRenderable
    {
        string AsXml();
        //XmlNode AsXml(XmlDocument doc);
        void FromXml(string Xml);
        //XmlNode FromXml(XmlNode node);
    }
    /// <summary>
    /// Used to allow for manipulation of xml but only for the classes within this assembly
    /// </summary>
    /// <seealso cref="EzDbSchema.Core.Interfaces.IXmlRenderable" />
    public interface IXmlRenderableInternal
    {
        XmlNode AsXml(XmlDocument doc);
        XmlNode FromXml(XmlNode node);
    }

}
