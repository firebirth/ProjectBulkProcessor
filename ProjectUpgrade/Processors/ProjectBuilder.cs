﻿using System.Xml;
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

        public ProjectBuilder()
        {
            _projectDocument = new XmlDocument();
            _root = GetRootElement(_projectDocument);
            _projectDocument.AppendChild(_root);
        }

        public IProjectGroupBuilder AddItemGroup() => AddGroupInternal("ItemGroup");

        public IProjectGroupBuilder AddPropertyGroup() => AddGroupInternal("PropertyGroup");

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

        public XDocument Build()
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