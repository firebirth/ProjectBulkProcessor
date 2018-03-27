﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Xml.Linq;
using ProjectUpgrade.Interfaces;
using ProjectUpgrade.Models;

namespace ProjectUpgrade.Processors
{
    public class ProjectParser : IProjectParser
    {
        public ProjectModel ParseProject(FileInfoBase projectFile)
        {
            XDocument doc;
            using (var fs = projectFile.OpenRead())
            {
                doc = XDocument.Load(fs);
            }

            var projectReferences = doc.Descendants("ProjectReference")
                                       .Select(e => e.Attribute("Include")?.Value)
                                       .Where(s => !string.IsNullOrEmpty(s))
                                       .Select(s => new ProjectReferenceModel(s))
                                       .ToImmutableList();

            var isExecutable = doc.Descendants("OutputType")
                                  .Any(x => string.Equals(x.Value, "Exe", StringComparison.OrdinalIgnoreCase));

            var packagesInfo = ParseDependencies(projectFile).ToImmutableList();

            return new ProjectModel(projectReferences, packagesInfo, isExecutable, projectFile);
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

            return doc.Descendants("package")
                      .Select(e => (Id: e.Attribute("id")?.Value, Version: e.Attribute("version")?.Value))
                      .Where(t => !string.IsNullOrEmpty(t.Id) && !string.IsNullOrEmpty(t.Version))
                      .Select(t => new PackageDependencyModel(t.Id, t.Version));
        }
    }
}
