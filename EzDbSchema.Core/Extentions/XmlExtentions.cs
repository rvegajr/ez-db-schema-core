using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace EzDbSchema.Core.Extentions
{
    public static class XmlExtentions
    {
        public static XmlNode Append(this XmlNode nod, string ElementName, string Value)
        {
            XmlNode newnod = nod.OwnerDocument.CreateElement(ElementName);
            newnod.InnerText = Value.ToSafeString();
            return nod.AppendChild(newnod);
        }

        /// <summary>
        /// Returns an item as an XML Node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="_doc">The document.</param>
        /// <param name="ElementName">Name of the element.</param>
        /// <param name="KeyAttributeValue">The key attribute value.</param>
        /// <returns></returns>
        public static XmlNode AsXmlNode<T>(this T item, XmlDocument _doc, string ElementName)
        {
            return item.AsXmlNode(_doc, ElementName, null);
        }
            
        /// <summary>
        /// Returns an item as an XML Node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="_doc">The document.</param>
        /// <param name="ElementName">Name of the element.</param>
        /// <param name="KeyAttributeValue">The key attribute value.</param>
        /// <returns></returns>
        public static XmlNode AsXmlNode<T>(this T item, XmlDocument _doc, string ElementName, string KeyAttributeValue )
        {
            try
            {
                if (item == null) return null;
                if (item.GetType().Name.Equals("String"))
                {
                    XmlNode singleNode = _doc.CreateElement(ElementName);
                    singleNode.InnerText = item.ToSafeString();
                    return singleNode;
                }
                XmlNode nod = _doc.CreateElement(XmlConvert.EncodeName(ElementName));
                if (!string.IsNullOrEmpty(KeyAttributeValue))  ((XmlElement)nod).SetAttribute("key", KeyAttributeValue);

                foreach (PropertyInfo pi in item.GetType().GetProperties())
                {
                    var isIndexProperty = (pi.GetIndexParameters().Length > 0);
                    if (!isIndexProperty) //ignore indexer properties
                    {
                        var asRefAttribute = (AsRef)pi.GetCustomAttribute(typeof(AsRef));
                        if (asRefAttribute != null)
                        {
                            try
                            {
                                var idValue = item.GetType().GetProperty(asRefAttribute.ReferenceFieldName).GetValue(item, null);
                                if (idValue!=null)
                                {
                                    XmlNode refnod = nod.OwnerDocument.CreateElement(pi.Name);
                                    ((XmlElement)refnod).SetAttribute("ref", idValue.ToSafeString());
                                    nod.AppendChild(refnod);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(string.Format("Error while getting reference value for {0}", asRefAttribute.ReferenceFieldName), ex);
                            }
                        }
                        else { 
                            var val = pi.GetValue(item, null);
                            IXmlRenderable valToRender = val as IXmlRenderable;
                            if (valToRender != null)
                            {
                                nod.AppendChild(valToRender.AsXml(_doc));
                            }
                            else if (((pi.PropertyType.FullName.Contains("DateTime") || pi.PropertyType.Name.Equals("String") || pi.PropertyType.IsPrimitive)) && (val != null))
                            {
                                XmlNode newnod = nod.OwnerDocument.CreateElement(pi.Name);
                                newnod.InnerText = val.ToSafeString();
                                nod.AppendChild(newnod);
                            }
                            else
                            {

                            }
                        }
                    }
                }
                return nod;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public static XmlNode ListAsXmlNode<T>(this List<T> items, XmlDocument _doc, string ElementName)
        {
            XmlNode nod = _doc.CreateElement(XmlConvert.EncodeName(ElementName));
            foreach (var item in items)
            {
                IXmlRenderable itemToRender = item as IXmlRenderable;
                if (itemToRender != null)
                {
                    nod.AppendChild(itemToRender.AsXmlNode(_doc, item.GetType().Name));
                } else
                {
                    nod.AppendChild(item.AsXmlNode(_doc, "item"));
                }
            }
            return nod;
        }
        /*
        public static XmlNode AppendDictionary<K, T>(this XmlNode _nod, Dictionary<K, T> items)
        {
            var nod = _nod;
            foreach (var itemKV in items)
            {
                nod.Append(itemKV.Key.ToString(), itemKV.Value);
            }
            return nod;
        }
        */
        public static XmlNode DictionaryAsXmlNode<T>(this Dictionary<string, T> items, XmlDocument _doc, string ElementName)
        {
            XmlNode nod = _doc.CreateElement(XmlConvert.EncodeName(ElementName));
            foreach (var itemKV in items)
            {
                IXmlRenderable valToRender = itemKV.Value as IXmlRenderable;
                if (valToRender != null)
                {
                    var newNode = nod.AppendChild(valToRender.AsXml(_doc));
                    ((XmlElement)newNode).SetAttribute("key", itemKV.Key);
                } else
                {
                    nod.AppendChild(itemKV.Value.AsXmlNode(_doc, itemKV.Value.GetType().Name, itemKV.Key));
                }
            }
            return nod;
            //return nod.AppendDictionary<string, T>(item);
        }

    }
}
