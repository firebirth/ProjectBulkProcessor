using System.Xml;
using System.Xml.Linq;
using ProjectUpgrade.Interfaces;

namespace ProjectUpgrade.Processors
{
    public class ProjectBuilder : IProjectElementBuilder, IProjectGroupBuilder
    {
        private readonly XmlDocument _projectDocument;
        private readonly XmlElement _root;
        private XmlElement _currentGroup;
        private XmlElement _currentElement;

        public static IProjectBuilder CreateProject(XmlDocument document = null)
        {
            var projectDocument = document ?? new XmlDocument();

            return new ProjectBuilder(projectDocument);
        }

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
