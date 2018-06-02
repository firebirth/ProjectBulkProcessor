using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ProjectBulkProcessor.Extensions
{
    public static class XmlElementExtensions
    {
        private static readonly XmlNamespaceManager ProjectNamespaceManager;

        static XmlElementExtensions()
        {
            ProjectNamespaceManager = new XmlNamespaceManager(new NameTable());
            ProjectNamespaceManager.AddNamespace("project", "http://schemas.microsoft.com/developer/msbuild/2003");
        }

        public static XElement GetProjectElementByName(this XNode xElement, string name)
        {
            return xElement.XPathSelectElement($"//project:{name}", ProjectNamespaceManager);
        }

        public static IEnumerable<XElement> GetProjectElementsByName(this XNode xElement, string name)
        {
            return xElement.XPathSelectElements($"//project:{name}", ProjectNamespaceManager);
        }
    }
}
