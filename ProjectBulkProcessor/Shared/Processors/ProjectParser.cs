using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using ProjectBulkProcessor.Extensions;
using ProjectBulkProcessor.Shared.Interfaces;
using ProjectBulkProcessor.Shared.Models;

namespace ProjectBulkProcessor.Shared.Processors
{
    public class ProjectParser : IProjectParser
    {
        public ProjectModel ParseProject(FileInfoBase projectFile)
        {
            XDocument doc;
            using (var fs = projectFile.OpenRead())
            {
                try
                {
                    doc = XDocument.Load(fs);
                }
                catch
                {
                    return null;
                }
            }

            var projectReferences = doc.GetProjectElementsByName("ProjectReference")
                                       .Select(e => e.Attribute("Include")?.Value)
                                       .Where(s => !string.IsNullOrEmpty(s))
                                       .Select(s => new ProjectReferenceModel(s))
                                       .ToImmutableList();


            var packagesInfo = ParseDependencies(projectFile).ToImmutableList();

            return new ProjectModel(projectFile, projectReferences, packagesInfo);
        }

        private static IEnumerable<PackageDependencyModel> ParseDependencies(FileInfoBase projectFile)
        {
            var packagesFile = projectFile.Directory
                                          .GetFiles("packages.config", SearchOption.TopDirectoryOnly)
                                          .SingleOrDefault();
            if (packagesFile == null)
            {
                return Enumerable.Empty<PackageDependencyModel>();
            }

            XDocument doc;
            using (var fs = packagesFile.OpenRead())
            {
                doc = XDocument.Load(fs);
            }

            return doc.XPathSelectElements("//package")
                      .Select(e => (Id: e.Attribute("id")?.Value, Version: e.Attribute("version")?.Value))
                      .Where(t => !string.IsNullOrEmpty(t.Id) && !string.IsNullOrEmpty(t.Version))
                      .Select(t => new PackageDependencyModel(t.Id, t.Version));
        }
    }
}
