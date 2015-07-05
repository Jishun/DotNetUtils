using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace DotNetUtils
{
    public static class XmlExtensions
    {
        public static T GetNodeValue<T>(this XmlNode node, string xpath, T defaultVal = default(T))
        {
            try
            {
                var sel = node.SelectSingleNode(xpath);
                if (sel == null)
                {
                    return defaultVal;
                }
                try
                {
                    var conv = TypeDescriptor.GetConverter(typeof(T));
                    var ret = (T)conv.ConvertFromInvariantString(sel.InnerText);
                    return ret;
                }
                catch
                {
                    return defaultVal;
                }
            }
            catch (XPathException ex)
            {
                throw new ArgumentOutOfRangeException("xpath", ex);
            }
        }

        public static T GetNodeValue<T>(this XmlNode node, XmlNamespaceManager nsmgr, string xpath, T defaultVal = default(T))
        {
            try
            {
                var sel = node.SelectSingleNode(xpath, nsmgr);
                if (sel == null)
                {
                    return defaultVal;
                }
                try
                {
                    var conv = TypeDescriptor.GetConverter(typeof(T));
                    var ret = (T)conv.ConvertFromInvariantString(sel.InnerText);
                    return ret;
                }
                catch
                {
                    return defaultVal;
                }
            }
            catch (XPathException ex)
            {
                throw new ArgumentOutOfRangeException("xpath", ex);
            }
        }
    }
}
