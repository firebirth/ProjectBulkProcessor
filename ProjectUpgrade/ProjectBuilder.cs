using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Extensions.Configuration;

namespace ProjectUpgrade
{
    public class ProjectBuilder
    {
        private readonly IConfigurationRoot _configuration;
        private readonly XmlDocument _existingProject;
        private readonly XmlDocument _newProject;
        private readonly FileInfo _projectFile;
        private readonly XmlElement _root;

        public ProjectBuilder(FileInfo projectFile, IConfigurationRoot configuration)
        {
            if (projectFile?.Exists != true)
                throw new ArgumentException("Provided file doesn't exist", nameof(projectFile));

            _projectFile = projectFile;
            _configuration = configuration;

            _existingProject = new XmlDocument();
            using (var fs = _projectFile.OpenRead())
            {
                _existingProject.Load(fs);
            }

            ProjectReferences = _existingProject.GetElementsByTagName("ProjectReference")
                                                .OfType<XmlElement>()
                                                .Select(x => x.GetAttribute("Include"))
                                                .ToList();

            IsExecutable = _existingProject.GetElementsByTagName("OutputType")
                                           .OfType<XmlElement>()
                                           .Any(x => x.Value == "Exe");
            
            _newProject = new XmlDocument();
            _root = _newProject.CreateElement("Project");
            _root.SetAttribute("Sdk", "Microsoft.NET.Sdk");
            _newProject.AppendChild(_root);
        }

        private bool IsExecutable { get; }

        private List<string> ProjectReferences { get; }

        public ProjectBuilder GenerateProjectReferenceSection()
        {
            if (!ProjectReferences.Any())
                return this;

            var dependencyItemGroup = _newProject.CreateElement("ItemGroup");
            _root.AppendChild(dependencyItemGroup);

            foreach (var reference in ProjectReferences)
            {
                var childNode = _newProject.CreateElement("ProjectReference");
                childNode.SetAttribute("Include", reference);
                dependencyItemGroup.AppendChild(childNode);
            }

            return this;
        }

        public ProjectBuilder GenerateDependenciesSection()
        {
            var packagesFile = _projectFile.Directory
                                           .GetFiles("packages.config", SearchOption.TopDirectoryOnly)
                                           .SingleOrDefault();
            if (packagesFile == null)
                return this;

            var packagesInfo = _existingProject.GetElementsByTagName("package")
                                               .OfType<XmlElement>()
                                               .Select(x => new
                                               {
                                                   id = x.GetAttribute("x"),
                                                   version = x.GetAttribute("version")
                                               });

            var dependencyItemGroup = _newProject.CreateElement("ItemGroup");

            foreach (var info in packagesInfo)
            {
                var childNode = _newProject.CreateElement("PackageReference");
                childNode.SetAttribute("Version", info.version);
                childNode.SetAttribute("Include", info.id);
                dependencyItemGroup.AppendChild(childNode);
            }

            _root.AppendChild(dependencyItemGroup);

            return this;
        }
        
        public ProjectBuilder GenerateCommonSection()
        {
            var generalPropGroup = _newProject.CreateElement("PropertyGroup");
            _root.AppendChild(generalPropGroup);

            AddNodeFromConfig(generalPropGroup, "targetFramework", true);
            AddNodeFromConfig(generalPropGroup, "copyright");
            AddNodeFromConfig(generalPropGroup, "company");
            AddNodeFromConfig(generalPropGroup, "authors");
            AddNodeFromConfig(generalPropGroup, "description");
            AddNodeFromConfig(generalPropGroup, "packageLicenseUrl");
            AddNodeFromConfig(generalPropGroup, "packageProjectUrl");
            AddNodeFromConfig(generalPropGroup, "packageIconUrl");
            AddNodeFromConfig(generalPropGroup, "repositoryUrl");
            AddNodeFromConfig(generalPropGroup, "repositoryType");
            AddNodeFromConfig(generalPropGroup, "packageTags");
            AddNodeFromConfig(generalPropGroup, "packageReleaseNotes");
            AddNodeFromConfig(generalPropGroup, "packageId");
            AddNodeFromConfig(generalPropGroup, "version");
            AddNodeFromConfig(generalPropGroup, "product");

            if (IsExecutable)
            {
                var childNode = _newProject.CreateElement("OutputType");
                childNode.InnerText = "Exe";
                generalPropGroup.AppendChild(childNode);
            }

            return this;
        }

        public XmlDocument Build()
        {
            return _newProject;
        }

        private void AddNodeFromConfig(XmlNode parentElement, string configKey, bool throwIfMissing = false)
        {
            var nodeName = CapitalizeFirstLetter(configKey);
            var configValue = _configuration[configKey];
            if (configValue != null)
            {
                var childNode = _newProject.CreateElement(nodeName);
                childNode.InnerText = configValue;
                parentElement.AppendChild(childNode);
            }
            else if (throwIfMissing)
            {
                throw new Exception($"Required parameter {configKey} is missing.");
            }
        }

        private static string CapitalizeFirstLetter(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            var charArray = s.ToCharArray();
            charArray[0] = char.ToUpper(charArray[0]);
            return new string(charArray);
        }
    }
}