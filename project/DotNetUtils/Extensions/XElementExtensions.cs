using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.Xml.Serialization;
using System.Text;
using System.Xml.XPath;

namespace DotNetUtils
{
    public static class XElementExtensions
    {
        /// <summary>
        /// Throw exception if specified element does not match the name given
        /// </summary>
        /// <param name="element">element to verify</param>
        /// <param name="expectedName">expected name</param>
        public static void VerifyElementName(this XElement element, string expectedName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (expectedName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(expectedName));
            }

            if (element.Name != expectedName)
            {
                string message = "Verify XElement name: Expected element name to be <{0}>. <{1}> was encountered instead.".FormatInvariantCulture(expectedName, element.Name);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Verify if specified attribute exist in the element
        /// </summary>
        /// <param name="element">The XElement this is called against</param>
        /// <param name="expectedName">Name to verify</param>
        public static bool AttributePresent(this XElement element, string expectedName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (expectedName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(expectedName));
            }

            return element.Attribute(expectedName) != null;
        }

        /// <summary>
        /// Verify if specified attribute exist in the element
        /// </summary>
        /// <param name="element">The XElement this is called against</param>
        /// <param name="expectedName">Name to verify</param>
        public static bool ElementPresent(this XElement element, string expectedName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (expectedName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(expectedName));
            }
            return element.Element(expectedName) != null;
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>int?</returns>
        public static int? GetAttributeIntNullable(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (int?)element.Attribute(attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>int</returns>
        public static int GetAttributeInt(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (int)element.ValidateAndGetAttribute(attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>long?</returns>
        public static long? GetAttributeInt64Nullable(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (long?)element.Attribute(attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>long</returns>
        public static long GetAttributeInt64(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (long)element.ValidateAndGetAttribute(attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>decimal?</returns>
        public static decimal? GetAttributeDecimalNullable(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (decimal?)element.Attribute(attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>decimal</returns>
        public static decimal GetAttributeDecimal(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (decimal)element.ValidateAndGetAttribute(attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>float?</returns>
        public static float? GetAttributeFloatNullable(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (float?)element.Attribute(attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>float</returns>
        public static float GetAttributeFloat(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (float)element.ValidateAndGetAttribute(attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>double?</returns>
        public static double? GetAttributeDoubleNullable(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (double?)element.Attribute(attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>double</returns>
        public static double GetAttributeDouble(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (double)element.ValidateAndGetAttribute(attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>short</returns>
        public static short GetAttributeShort(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (short)element.ValidateAndGetAttribute(attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>Datetime?</returns>
        public static DateTime? GetAttributeDateTimeNullable(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (DateTime?)element.Attribute(attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>Datetime</returns>
        public static DateTime GetAttributeDateTime(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (DateTime)element.ValidateAndGetAttribute(attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>bool</returns>
        public static bool? GetAttributeBoolNullable(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            var value = (string)element.Attribute(attributeName);
            if (null == value)
            {
                return null;
            }
            return GetAttributeBool(element, attributeName);
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>bool</returns>
        public static bool GetAttributeBool(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            var value = (string)element.Attribute(attributeName);

            if (value != null)
            {
                switch (value.ToLower())
                {
                    case "0":
                    case "false":
                    case "off":
                    case "no":
                        return false;

                    case "1":
                    case "true":
                    case "on":
                    case "yes":
                        return true;
                }
            }
            throw new FormatException("Failed to get bool value from specified attribute: {0}".FormatInvariantCulture(attributeName));
        }

        /// <summary>
        /// get specified attribute value and cast to specified type
        /// </summary>
        /// <param name="element">element src</param>
        /// <param name="attributeName">attribute name</param>
        /// <returns>string</returns>
        public static string GetAttributeString(this XElement element, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            return (string)element.Attribute(attributeName);
        }

        public static XAttribute ValidateAndGetAttribute(this XElement element, string attributeName)
        {
            if (attributeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(attributeName));
            }
            var att = element.Attribute(attributeName);
            if (null == att)
            {
                throw new InvalidDataException("The element does not contain attribute named '{0}'".FormatInvariantCulture(attributeName));
            }
            return att;
        }

        /// <summary>
        /// Checks whether the XElement is null or contains no items.
        /// </summary>
        /// <param name="element">The instance of XElement</param>
        /// <returns>True if the element is null or empty.</returns>
        public static bool IsNullOrEmpty(this XElement element)
        {
            return ((element == null) || (element.IsEmpty) || (!element.Elements().Any()));
        }

        /// <summary>
        /// To string function to avoid null reference exception
        /// </summary>
        /// <param name="element">The instance of XElement</param>
        /// <returns>element string, null if the element is null</returns>
        public static string SafeToString(this XElement element)
        {
            return element == null ? null : element.ToString();
        }

        /// <summary>
        /// Get value function to avoid null reference exception
        /// </summary>
        /// <param name="element">The instance of XElement</param>
        /// <returns>element value, null if the element is null</returns>
        public static string SafeGetValue(this XElement element)
        {
            return element == null ? null : element.Value;
        }

        /// <summary>
        /// Get value function to avoid null reference exception
        /// </summary>
        /// <param name="root">The instance of XElement</param>
        /// <param name="desendElementName">The descendant element name</param>
        /// <param name="ns">The namespace</param>
        /// <returns>element value, null if the element is null</returns>
        public static string SafeGetDescendantElementValue(this XElement root, string desendantElementName, XNamespace ns)
        {
            if (desendantElementName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("desendantElementName");
            }
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            var targetElement = root.Descendants(ns + desendantElementName).FirstOrDefault();
            return targetElement == null ? null : targetElement.Value;
        }

        public static T GetElementValue<T>(this XElement root, XNamespace ns, T defaultVal = default(T), params string[] names)
        {
            if (names.IsNullOrEmpty())
            {
                throw new ArgumentNullException("names");
            }

            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            var node = root;
            foreach (string name in names)
            {
                node = node.Descendants(ns + name).FirstOrDefault();
                if (node == null)
                {
                    break;
                }
            }

            if (node == null)
            {
                return defaultVal;
            }

            try
            {
                var conv = TypeDescriptor.GetConverter(typeof(T));
                var ret = (T)conv.ConvertFromInvariantString(node.Value);
                return ret;
            }
            catch
            {
                return defaultVal;
            }
        }

        /// <summary>
        /// Get DateTime value from descendant element
        /// </summary>
        /// <param name="root">The instance of XElement</param>
        /// <param name="desendantElementName">The descendant element name</param>
        /// <param name="ns">The namespace</param>
        /// <returns>The DateTime instance</returns>
        public static DateTime GetDescendantElementDateTimeValue(this XElement root, string desendantElementName, XNamespace ns)
        {
            var value = root.SafeGetDescendantElementValue(desendantElementName, ns);
            if (value == null)
            {
                throw new ArgumentException("Cannot load descendant element {0}".FormatInvariantCulture(desendantElementName), nameof(desendantElementName));
            }
            var dateTime = default(DateTime);
            if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTime))
            {
                throw new ArgumentException("value {0} is not a valid date time value".FormatInvariantCulture(value), "value");
            }
            return dateTime;
        }

        /// <summary>
        /// Get DateTime value from descendant element
        /// </summary>
        /// <param name="root">The instance of XElement</param>
        /// <param name="desendantElementName">The descendant element name</param>
        /// <param name="ns">The namespace</param>
        /// <returns>The DateTime instance</returns>
        public static DateTime? GetDescendantElementNullableDateTimeValue(this XElement root, string desendantElementName, XNamespace ns)
        {
            var value = root.SafeGetDescendantElementValue(desendantElementName, ns);
            if (value == null)
            {
                return null;
            }
            var dateTime = default(DateTime);
            if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTime))
            {
                throw new ArgumentException("value {0} is not a valid date time value".FormatInvariantCulture(value), "value");
            }
            return dateTime;
        }

        /// <summary>
        /// Get decimal value from descendant element
        /// </summary>
        /// <param name="root">The instance of XElement</param>
        /// <param name="desendantElementName">The descendant element name</param>
        /// <param name="ns">The namespace</param>
        /// <returns>The decimal instance</returns>
        public static decimal GetDescendantElementDecimalValue(this XElement root, string desendantElementName, XNamespace ns)
        {
            var value = root.SafeGetDescendantElementValue(desendantElementName, ns);
            if (value.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("cannot get value for element {0}".FormatInvariantCulture(desendantElementName));
            }
            if (value.StartsWith("."))
            {
                value = "0" + value;
            }
            var decimalValue = default(decimal);
            if (!decimal.TryParse(value, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimalValue))
            {
                throw new ArgumentException("value {0} is not a valid decimal value".FormatInvariantCulture(value), "value");
            }
            return decimalValue;
        }

        /// <summary>
        /// Get nullable decimal value from descendant element
        /// </summary>
        /// <param name="root">The instance of XElement</param>
        /// <param name="desendantElementName">The descendant element name</param>
        /// <param name="ns">The namespace</param>
        /// <returns>The Nullable{decimal} instance</returns>
        public static decimal? GetDescendantElementNullableDecimalValue(this XElement root, string desendantElementName, XNamespace ns)
        {
            var value = root.SafeGetDescendantElementValue(desendantElementName, ns);
            var decimalValue = default(decimal);
            return value == null || !decimal.TryParse(value, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimalValue) ? null : (decimal?)decimalValue;
        }

        /// <summary>
        /// Get boolean value from descendant element
        /// </summary>
        /// <param name="root">The instance of XElement</param>
        /// <param name="desendantElementName">The descendant element name</param>
        /// <param name="ns">The namespace</param>
        /// <returns>The boolean instance</returns>
        public static bool GetDescendantElementBooleanValue(this XElement root, string desendantElementName, XNamespace ns, bool treatNonExistsAsFalse = false)
        {
            var value = root.SafeGetDescendantElementValue(desendantElementName, ns);
            if (value == null)
            {
                if (treatNonExistsAsFalse)
                {
                    return false;
                }
                throw new ArgumentException("Cannot load descendant element {0}".FormatInvariantCulture(desendantElementName), nameof(desendantElementName));
            }
            if (value.Equals("Y", StringComparison.OrdinalIgnoreCase) || value.Equals("X", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (value.Equals("N", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            var booleanValue = default(bool);
            if (!bool.TryParse(value, out booleanValue))
            {
                throw new ArgumentException("value {0} is not a valid boolean value".FormatInvariantCulture(value), "value");
            }
            return booleanValue;
        }

        /// <summary>
        /// Get integer value from descendant element
        /// </summary>
        /// <param name="root">The instance of XElement</param>
        /// <param name="desendantElementName">The descendant element name</param>
        /// <param name="ns">The namespace</param>
        /// <returns>The integer value</returns>
        public static int GetDescendantElementInt32Value(this XElement root, string desendantElementName, XNamespace ns)
        {
            var value = root.SafeGetDescendantElementValue(desendantElementName, ns);
            if (value == null)
            {
                throw new ArgumentException("Cannot load descendant element {0}".FormatInvariantCulture(desendantElementName), "desendantElementName");
            }
            var integerValue = default(int);
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out integerValue))
            {
                throw new ArgumentException("value {0} is not a valid integer value".FormatInvariantCulture(value), "value");
            }
            return integerValue;
        }

        /// <summary>
        /// Get Decendants by name with ignoring name space
        /// </summary>
        public static IEnumerable<XElement> GetDecendants(this XElement node, string name)
        {
            if (node == null)
            {
                return null;
            }
            return node.Descendants().Where(n => n.Name.LocalName == name);
        }

        /// <summary>
        /// Get Decendants by name with ignoring name space
        /// </summary>
        public static IEnumerable<XElement> GetDecendants(this XDocument node, string name)
        {
            if (node == null)
            {
                return null;
            }
            return node.Descendants().Where(n => n.Name.LocalName == name);
        }

        /// <summary>
        /// Serialize an object to XElement instance
        /// </summary>
        /// <typeparam name="T">The type to be serialized</typeparam>
        /// <param name="obj">The object to be serialized</param>
        /// <returns>An XElement instance represent object</returns>
        public static XElement ToXElement<T>(this T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(streamWriter, obj);
                    return XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));
                }
            }
        }

        /// <summary>
        /// De serialize xml the xml using XmlSerializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xElement"></param>
        /// <returns></returns>
        public static T FromXElement<T>(this XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(element.ToString())))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(memoryStream);
            }
        }

        /// <summary>
        /// Remove namespaces in the exlement and its descendants
        /// </summary>
        public static void RemoveXnameSpaces(this XElement element)
        {
            foreach (var x in element.DescendantsAndSelf())
            {
                x.Name = x.Name.LocalName;
                x.ReplaceAttributes((from xattrib in x.Attributes().Where(xa => !xa.IsNamespaceDeclaration) select new XAttribute(xattrib.Name.LocalName, xattrib.Value)));
            }
        }

        public static void RemoveWithNextWhitespace(this XElement element)
        {
            IEnumerable<XText> textNodes
                = element.NodesAfterSelf()
                         .TakeWhile(node => node is XText).Cast<XText>();
            if (element.ElementsAfterSelf().Any())
            {
                // Easy case, remove following text nodes.
                textNodes.ToList().ForEach(node => node.Remove());
            }
            else
            {
                // Remove trailing whitespace.
                textNodes.TakeWhile(text => !text.Value.Contains("\n"))
                         .ToList().ForEach(text => text.Remove());
                // Fetch text node containing newline, if any.
                XText newLineTextNode
                    = element.NodesAfterSelf().OfType<XText>().FirstOrDefault();
                if (newLineTextNode != null)
                {
                    string value = newLineTextNode.Value;
                    if (value.Length > 1)
                    {
                        // Composite text node, trim until newline (inclusive).
                        newLineTextNode.AddAfterSelf(
                            new XText(value.Substring(value.IndexOf('\n') + 1)));
                    }
                    // Remove original node.
                    newLineTextNode.Remove();
                }
            }
            element.Remove();
        }

        public static void MoveFirst(this XElement element)
        {
            var p = element.Parent;
            element.Remove();
            p.AddFirst(element);
        }

        public static void MoveLast(this XElement element)
        {
            var p = element.Parent;
            element.Remove();
            p.Add(element);
        }

        /// <summary>
        /// insert the 'toAdd' element before this element, with option to skip count
        /// </summary>
        /// <param name="element"></param>
        /// <param name="toAdd"></param>
        /// <param name="skip"></param>
        public static void InsertElementBefore(this XElement element, XElement toAdd, int skip = 0)
        {
            XNode node = element;
            while (element.PreviousNode != null && skip > 0)
            {
                node = element.PreviousNode;
                if (node is XElement)
                {
                    skip--;
                }
            }
            ((XElement)node).AddBeforeSelf(toAdd);
        }

        /// <summary>
        /// insert the 'toAdd' element after this element, with option to skip count
        /// </summary>
        /// <param name="element"></param>
        /// <param name="toAdd"></param>
        /// <param name="skip"></param>
        public static void InsertElementAfter(this XElement element, XElement toAdd, int skip = 0)
        {
            XNode node = element;
            while (element.NextNode != null && skip > 0)
            {
                node = element.NextNode;
                if (node is XElement)
                {
                    skip--;
                }
            }
            ((XElement)node).AddAfterSelf(toAdd);
        }
    }
}
