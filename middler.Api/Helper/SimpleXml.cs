using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Reflectensions.ExtensionMethods;

namespace middler.Api.Helper
{
    public class SimpleXmlBuilder
    {
        public SimpleXml FromString(string xml) => new SimpleXml(xml);
    }

    public class SimpleXml
    {
        internal XDocument Document { get; }
        internal XmlNamespaceManager NamespaceManager { get; set; } = new XmlNamespaceManager(new NameTable());

        public SimpleXml()
        {
            Document = new XDocument();
        }

        public SimpleXml(string xml)
        {
            Document = XDocument.Parse(xml);
        }

        public SimpleXml(Stream xml)
        {
            Document = XDocument.Load(xml);
        }

        public SimpleXml(SimpleXmlElement element)
        {
            Document = XDocument.Parse(element.ToString());
        }

        public SimpleXml(XDocument element)
        {
            Document = element;
        }


        public SimpleXmlElement[] SelectElements(string xpath)
        {

            var elements = Document.XPathSelectElements(xpath, NamespaceManager);
            var list = new List<SimpleXmlElement>();
            foreach (var xElement in elements)
            {
                list.Add(new SimpleXmlElement(xElement).AddNameSpaceManager(NamespaceManager));
            }
            return list.ToArray();
        }

        public SimpleXmlElement[] SelectElements()
        {

            return Document.Elements().Select(xElement =>
                new SimpleXmlElement(xElement).AddNameSpaceManager(NamespaceManager)).ToArray();

        }

        public SimpleXmlElement SelectElement(string xpath)
        {
            var el = Document.XPathSelectElement(xpath, NamespaceManager);
            if (el == null)
                return null;

            return new SimpleXmlElement(el).AddNameSpaceManager(NamespaceManager);
        }

        public string GetAttribute(string name)
        {
            return new SimpleXmlElement(Document.Root).GetAttribute(name);
        }

        public Dictionary<string, string> GetAttributes()
        {
            return new SimpleXmlElement(Document.Root).GetAttributes();
        }

        public SimpleXml RemoveComments()
        {
            Document.DescendantNodes().OfType<XComment>().Remove();
            return this;
        }

        public override string ToString()
        {
            return Document.ToString();
        }


        public SimpleXml AddNameSpace(string prefix, string uri)
        {
            NamespaceManager.AddNamespace(prefix, uri);
            return this;
        }

        public SimpleXml AddNameSpaceManager(XmlNamespaceManager namespaceManager)
        {
            this.NamespaceManager = namespaceManager;
            return this;
        }


        public static SimpleXml Parse(string value)
        {
            return new SimpleXml(value);
        }

        public static SimpleXml Parse(SimpleXmlElement element)
        {
            return new SimpleXml(element);
        }

        public static implicit operator XDocument(SimpleXml simpleXml)
        {
            return simpleXml.Document;
        }
    }

    public class SimpleXmlElement
    {


        internal XElement Element { get; }
        internal XmlNamespaceManager NamespaceManager { get; set; } = new XmlNamespaceManager(new NameTable());

        public string Name => Element.Name.LocalName;

        public SimpleXmlElement(string xml)
        {
            Element = XElement.Parse(xml);
        }
        public SimpleXmlElement(XElement element)
        {
            Element = element;
        }

        public string GetValue()
        {
            return Element?.Value;
        }

        public bool? GetValueAsBoolean()
        {
            var el = Element?.Value;
            if (el == null)
                return null;

            return el.To<bool>();
        }

        public SimpleXmlElement[] SelectElements()
        {

            return Element.Elements().Select(el => new SimpleXmlElement(el).AddNameSpaceManager(NamespaceManager)).ToArray();

        }

        public SimpleXmlElement[] SelectElements(string xpath)
        {
            var elements = Element.XPathSelectElements(xpath, NamespaceManager);
            var list = new List<SimpleXmlElement>();
            foreach (var xElement in elements)
            {
                list.Add(new SimpleXmlElement(xElement).AddNameSpaceManager(NamespaceManager));
            }
            return list.ToArray();
        }

        public SimpleXmlElement SelectElement(string xpath)
        {
            var el = Element.XPathSelectElement(xpath, NamespaceManager);
            if (el == null)
                return null;

            return new SimpleXmlElement(el).AddNameSpaceManager(NamespaceManager);
        }

        public string GetAttribute(string name)
        {
            var attr = Element.Attribute(name);
            return attr?.Value;
        }

        public Dictionary<string, string> GetAttributes()
        {

            var attr = Element.Attributes()
                .Select(at => new KeyValuePair<string, string>(at.Name.LocalName, at.Value));

            return new Dictionary<string, string>(attr);
        }

        public SimpleXmlElement RemoveComments()
        {
            Element.DescendantNodes().OfType<XComment>().Remove();
            return this;
        }

        public SimpleXml ToSimpleXml()
        {
            return new SimpleXml(this).AddNameSpaceManager(NamespaceManager);
        }

        public override string ToString()
        {
            return Element?.ToString();
        }


        public SimpleXmlElement AddNameSpace(string prefix, string uri)
        {
            NamespaceManager.AddNamespace(prefix, uri);
            return this;
        }

        public SimpleXmlElement AddNameSpaceManager(XmlNamespaceManager namespaceManager)
        {
            this.NamespaceManager = namespaceManager;
            return this;
        }


        public static SimpleXmlElement Parse(string value)
        {
            return new SimpleXmlElement(value);
        }


        public static implicit operator XDocument(SimpleXmlElement simpleXmlElement)
        {
            return simpleXmlElement.ToSimpleXml().Document;
        }
    }

}
