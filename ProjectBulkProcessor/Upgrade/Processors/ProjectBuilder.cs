using ProjectBulkProcessor.Upgrade.Interfaces;
using System.Xml;
using System.Xml.Linq;

namespace ProjectBulkProcessor.Upgrade.Processors
{
    public class ProjectBuilder : IProjectElementBuilder, IProjectGroupBuilder
    {
        private readonly XmlDocument _projectDocument;
        private readonly XmlElement _root;
        private XmlElement _currentGroup;
        private XmlElement _currentElement;

        public static IProjectBuilder CreateProject() => new ProjectBuilder(new XmlDocument());

        private ProjectBuilder(XmlDocument projectDocument)
        {
            _projectDocument = projectDocument;
            _root = GetRootElement(_projectDocument);
            _projectDocument.AppendChild(_root);
        }

        IProjectGroupBuilder IProjectBuilder.AddItemGroup() => AddGroupInternal("ItemGroup");

        IProjectGroupBuilder IProjectBuilder.AddPropertyGroup() => AddGroupInternal("PropertyGroup");

        IProjectElementBuilder IProjectGroupBuilder.WithElement(string elementName)
        {
            AttachHangingElements();

            _currentElement = _projectDocument.CreateElement(elementName);

            return this;
        }

        IProjectElementBuilder IProjectElementBuilder.WithNodeValue(string nodeValue)
        {
            _currentElement.InnerText = nodeValue;

            return this;
        }

        IProjectElementBuilder IProjectElementBuilder.WithAttribute(string attributeName, string attributeValue)
        {
            _currentElement.SetAttribute(attributeName, attributeValue);

            return this;
        }

        XDocument IProjectBuilder.Build()
        {
            AttachHangingElements();

            foreach (XmlElement node in _root.ChildNodes)
            {
                if (!node.HasChildNodes && !node.HasAttributes)
                {
                    _root.RemoveChild(node);
                }
            }

            using (var reader = new XmlNodeReader(_projectDocument))
            {
                return XDocument.Load(reader);
            }
        }

        private IProjectGroupBuilder AddGroupInternal(string groupName)
        {
            AttachHangingElements();

            _currentGroup = _projectDocument.CreateElement(groupName);

            return this;
        }

        private void AttachHangingElements()
        {
            // TODO: check if this doesn't add duplicates
            if (_currentElement != null)
            {
                _currentGroup.AppendChild(_currentElement);
                _currentElement = null;
            }

            if (_currentGroup != null)
            {
                _root.AppendChild(_currentGroup);
            }
        }

        private static XmlElement GetRootElement(XmlDocument document)
        {
            var root = document.CreateElement("Project");
            root.SetAttribute("Sdk", "Microsoft.NET.Sdk");
            return root;
        }
    }
}
