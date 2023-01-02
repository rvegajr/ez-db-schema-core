using EzDbSchema.Core.Extentions.Objects;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("EzDbSchema.MsSql")]

namespace EzDbSchema.Core.Extentions.Xml
{
    internal static class XmlExtentions
    {
        internal static XmlNode Append(this XmlNode nod, string ElementName, string Value)
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
        internal static XmlNode AsXmlNode<T>(this T item, XmlDocument _doc, string ElementName)
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
        internal static XmlNode AsXmlNode<T>(this T item, XmlDocument _doc, string ElementName, string KeyAttributeValue )
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
                                var _refVal = pi.GetValue(item, null);
                                IEzObject refVal = _refVal as IEzObject;
                                var idValue = refVal.GetType().GetProperty(asRefAttribute.ReferenceFieldName).GetValue(refVal, null);
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
                                nod.AppendChild(((IXmlRenderableInternal)valToRender).AsXml(_doc));
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
        
        internal static XmlNode ListAsXmlNode<T>(this List<T> items, XmlDocument _doc, string ElementName)
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

        internal static XmlNode DictionaryAsXmlNode<T>(this Dictionary<string, T> items, XmlDocument _doc, string ElementName)
        {
            XmlNode nod = _doc.CreateElement(XmlConvert.EncodeName(ElementName));
            foreach (var itemKV in items)
            {
                IXmlRenderable valToRender = itemKV.Value as IXmlRenderable;
                if (valToRender != null)
                {
                    var newNode = nod.AppendChild(((IXmlRenderableInternal)valToRender).AsXml(_doc));
                    ((XmlElement)newNode).SetAttribute("key", itemKV.Key);
                } else
                {
                    nod.AppendChild(itemKV.Value.AsXmlNode(_doc, itemKV.Value.GetType().Name, itemKV.Key));
                }
            }
            return nod;
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
        internal static void FromXmlNode<T>(this T item, XmlNode node, string ElementName)
        {
            try
            {
                if (node.Name.Equals(ElementName))
                {
                    foreach (XmlElement ele in node.ChildNodes)
                    {
                        try
                        {
                            var pi = item.GetType().GetProperty(ele.Name);
                            if (pi!=null)
                            {
                                var refId = (ele.Attributes["ref"] != null ? ele.Attributes["ref"].InnerText : null);
                                var target = pi.GetValue(item, null);
                                var isNullable = (Nullable.GetUnderlyingType(pi.PropertyType) != null);
                                var propertyType = (!isNullable ? pi.PropertyType : Nullable.GetUnderlyingType(pi.PropertyType));
                                if (!string.IsNullOrEmpty(refId))
                                {
                                    //Create a negative reference id so we will come back and resolve this later
                                    var o = (IEzObject)EzObject.CreateInstance(propertyType.Name);
                                    o._id = int.Parse(refId) * -1;
                                    pi.SetValue(item, o);
                                } else {
                                    var val = ele.InnerText;

                                    if (((propertyType.FullName.Contains("DateTime") || propertyType.Name.Equals("String") || propertyType.IsPrimitive)) && (!string.IsNullOrWhiteSpace(val)))
                                    {
                                        if (propertyType.FullName.Contains("DateTime"))
                                        {
                                            DateTime value = DateTime.Parse(val);
                                            pi.SetValue(item, value);
                                        }
                                        else
                                        {
                                            var value = Convert.ChangeType(ele.InnerText, pi.PropertyType);
                                            pi.SetValue(item, value);
                                        }
                                    }
                                    else if (target != null)
                                    {
                                        IXmlRenderable valToRender = target as IXmlRenderable;
                                        if (valToRender != null)
                                        {
                                            try
                                            {
                                                ((IXmlRenderableInternal)valToRender).FromXml(ele);
                                            }
                                            catch (Exception)
                                            {
                                                throw;
                                            }
                                        }
                                    }
                                }
                                if (ele.Name.Equals("_id"))
                                    ((IEzObject)item).SetRefId();
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        internal static void DictionaryFromXmlNodeList<T>(this Dictionary<string, T> items, XmlNodeList nodelist, string ElementName)
        {
            foreach (XmlElement ele in nodelist)
            {
                var name = ele.Name;
                var key = ele.Attributes["key"].InnerText;
                var obj = EzObject.CreateInstance(ele.Name);
                IXmlRenderable valToRender = obj as IXmlRenderable;
                if (valToRender != null)
                {
                    ((IXmlRenderableInternal)valToRender).FromXml(ele);
                    var e = (T)valToRender;
                    items.Add(key, e);
                }
            }
        }

        internal static void ListFromXmlNodeList<T>(this List<T> items, XmlNodeList nodelist, string ElementName)
        {
            foreach (XmlElement ele in nodelist)
            {
                var name = ele.Name;
                var obj = EzObject.CreateInstance(ele.Name);
                IXmlRenderable valToRender = obj as IXmlRenderable;
                if (valToRender != null)
                {
                    T e = default(T);
                    if ((ele.Attributes["ref"] != null) && (ele.Attributes["ref"].InnerText.Length>0))
                    {
                        e = (T)EzObject.CreateInstance(ele.Name);
                        ((IEzObject)e)._id = (int.Parse(ele.Attributes["ref"].InnerText) * -1);
                    } else
                    {
                        ((IXmlRenderableInternal)valToRender).FromXml(ele);
                        e = (T)valToRender;
                    }
                    items.Add(e);
                }
            }
        }
    }
}
