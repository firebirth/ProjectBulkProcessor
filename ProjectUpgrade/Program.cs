using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

namespace ProjectUpgrade
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        public static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            _configuration = configBuilder.Build();

            foreach (var projectFile in GetProjectFiles())
            {
                var newProject = GenerateProject(projectFile);

                using (var fs = new FileStream(projectFile.FullName, FileMode.Truncate))
                {
                    newProject.Save(fs);
                }
            }
        }

        private static IEnumerable<FileInfo> GetProjectFiles()
        {
            return Directory.GetFiles(_configuration["rootPath"], "*.csproj", SearchOption.AllDirectories)
                            .Select(p => new FileInfo(p));
        }

        private static bool IsExecutable(FileInfo projectFile)
        {
            var xmlDoc = new XmlDocument();
            using (var fs = projectFile.OpenRead())
            {
                xmlDoc.Load(fs);
            }

            return xmlDoc.GetElementsByTagName("OutputType").OfType<XmlElement>().Any(x => x.Value == "Exe");
        }

        private static List<string> GetProjectReferences(FileInfo projectFile)
        {
            var xmlDoc = new XmlDocument();
            using (var fs = projectFile.OpenRead())
            {
                xmlDoc.Load(fs);
            }

            return xmlDoc.GetElementsByTagName("ProjectReference").OfType<XmlElement>().Select(x=>x.GetAttribute("Include")).ToList();
        }

        private static XmlDocument GenerateProject(FileInfo projectFile)
        {
            var xmlDoc = new XmlDocument();

            var projectNode = xmlDoc.CreateElement("Project");
            projectNode.SetAttribute("Sdk", "Microsoft.NET.Sdk");
            xmlDoc.AppendChild(projectNode);

            GenerateCommonSection(projectFile, xmlDoc, projectNode);

            GenerateDependenciesSection(projectFile, xmlDoc, projectNode);

            GenerateProjectReferenceSection(projectFile, xmlDoc, projectNode);

            return xmlDoc;
        }

        private static void GenerateProjectReferenceSection(FileInfo projectFile, XmlDocument xmlDoc, XmlElement projectNode)
        {
            var projectReferences = GetProjectReferences(projectFile);
            if (!projectReferences.Any())
            {
                return;
            }

            var dependencyItemGroup = xmlDoc.CreateElement("ItemGroup");
            projectNode.AppendChild(dependencyItemGroup);

            foreach (var reference in projectReferences)
            {
                var childNode = xmlDoc.CreateElement("ProjectReference");
                childNode.SetAttribute("Include", reference);
                dependencyItemGroup.AppendChild(childNode);
            }
        }

        private static void GenerateDependenciesSection(FileInfo projectFile, XmlDocument xmlDoc, XmlElement projectNode)
        {
            var packagesFile = projectFile.Directory.GetFiles("packages.config", SearchOption.TopDirectoryOnly)
                                          .FirstOrDefault();
            if (packagesFile == null)
            {
                return;
            }

            using (var s = packagesFile.OpenRead())
            {
                var packageInfo = XDocument.Load(s).Element("packages").Descendants()
                                           .Select(x => new
                                           {
                                               id = x.Attribute("id").Value,
                                               version = x.Attribute("version").Value
                                           });

                var dependencyItemGroup = xmlDoc.CreateElement("ItemGroup");
                projectNode.AppendChild(dependencyItemGroup);

                foreach (var info in packageInfo)
                {
                    var childNode = xmlDoc.CreateElement("PackageReference");
                    childNode.SetAttribute("Version", info.version);
                    childNode.SetAttribute("Include", info.id);
                    dependencyItemGroup.AppendChild(childNode);
                }
            }
        }

        private static void GenerateCommonSection(FileInfo projectFile, XmlDocument xmlDoc, XmlNode projectNode)
        {
            var generalPropGroup = xmlDoc.CreateElement("PropertyGroup");
            projectNode.AppendChild(generalPropGroup);

            AddValueFromConfig(xmlDoc, generalPropGroup, "targetFramework", true);
            AddValueFromConfig(xmlDoc, generalPropGroup, "copyright");
            AddValueFromConfig(xmlDoc, generalPropGroup, "company");
            AddValueFromConfig(xmlDoc, generalPropGroup, "authors");
            AddValueFromConfig(xmlDoc, generalPropGroup, "description");
            AddValueFromConfig(xmlDoc, generalPropGroup, "packageLicenseUrl");
            AddValueFromConfig(xmlDoc, generalPropGroup, "packageProjectUrl");
            AddValueFromConfig(xmlDoc, generalPropGroup, "packageIconUrl");
            AddValueFromConfig(xmlDoc, generalPropGroup, "repositoryUrl");
            AddValueFromConfig(xmlDoc, generalPropGroup, "repositoryType");
            AddValueFromConfig(xmlDoc, generalPropGroup, "packageTags");
            AddValueFromConfig(xmlDoc, generalPropGroup, "packageReleaseNotes");
            AddValueFromConfig(xmlDoc, generalPropGroup, "packageId");
            AddValueFromConfig(xmlDoc, generalPropGroup, "version");
            AddValueFromConfig(xmlDoc, generalPropGroup, "product");

            if (IsExecutable(projectFile))
            {
                var childNode = xmlDoc.CreateElement("OutputType");
                childNode.InnerText = "Exe";
                generalPropGroup.AppendChild(childNode);
            }
        }

        private static void AddValueFromConfig(XmlDocument xmlDoc, XmlNode parentElement, string configKey, bool throwIfMissing = false)
        {
            var nodeName = CapitalizeFirstLetter(configKey);
            var configValue = _configuration[configKey];
            if (configValue != null)
            {
                var childNode = xmlDoc.CreateElement(nodeName);
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
