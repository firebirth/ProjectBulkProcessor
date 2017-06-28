using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ProjectUpgrade.Models
{
    public class ProjectModel
    {
        public FileInfo ProjectFile { get; }
        public IReadOnlyCollection<ProjectReferenceModel> ProjectReferences { get; }
        public IReadOnlyCollection<ProjectDependencyModel> ProjectDependencies { get; }
        public bool IsExecutable { get; }
        
        private ProjectModel(FileInfo projectFile,
                             IReadOnlyCollection<ProjectReferenceModel> projectReferences,
                             IReadOnlyCollection<ProjectDependencyModel> projectDependencies,
                             bool isExecutable)
        {
            ProjectFile = projectFile;
            ProjectReferences = projectReferences;
            ProjectDependencies = projectDependencies;
            IsExecutable = isExecutable;
        }

        public static ProjectModel Parse(FileInfo projectFile)
        {
            var existingProject = new XmlDocument();
            using (var fs = projectFile.OpenRead())
            {
                existingProject.Load(fs);
            }

            var projectReferences = existingProject.GetElementsByTagName("ProjectReference")
                                                   .OfType<XmlElement>()
                                                   .Select(x => x.GetAttribute("Include"))
                                                   .Select(a => new ProjectReferenceModel(a))
                                                   .ToList();

            var isExecutable = existingProject.GetElementsByTagName("OutputType")
                                          .OfType<XmlElement>()
                                          .Any(x => x.InnerText == "Exe");

            var packagesInfo = ParseDependencies(projectFile);

            return new ProjectModel(projectFile, projectReferences, packagesInfo, isExecutable);
        }

        private static IReadOnlyCollection<ProjectDependencyModel> ParseDependencies(FileInfo projectFile)
        {
            var packagesFile = projectFile.Directory
                                          .GetFiles("packages.config", SearchOption.TopDirectoryOnly)
                                          .SingleOrDefault();
            if (packagesFile == null)
                return Enumerable.Empty<ProjectDependencyModel>().ToList();

            var packagesDoc = new XmlDocument();
            using (var fs = packagesFile.OpenRead())
            {
                packagesDoc.Load(fs);
            }

            return packagesDoc.GetElementsByTagName("package")
                              .OfType<XmlElement>()
                              .Select(x => new
                              {
                                  id = x.GetAttribute("id"),
                                  version = x.GetAttribute("version")
                              })
                              .Select(x => new ProjectDependencyModel(x.id, x.version))
                              .ToList();
        }
    }
}
