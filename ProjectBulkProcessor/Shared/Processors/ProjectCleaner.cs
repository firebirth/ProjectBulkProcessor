using ProjectBulkProcessor.Shared.Interfaces;
using ProjectBulkProcessor.Shared.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace ProjectBulkProcessor.Shared.Processors
{
    public class ProjectCleaner : IProjectCleaner
    {
        private const string AssemblyInfoFileName = "AssemblyInfo.cs";
        private const string PackagesConfigFileName = "packages.config";

        private readonly IFileSystem _fileSystem;

        public ProjectCleaner(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void DeleteDeprecatedFiles(string rootFolder)
        {
            var rootDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(rootFolder);

            var assemblyInfoFiles = rootDirectory.GetFiles(AssemblyInfoFileName, SearchOption.AllDirectories);
            var packageFiles = rootDirectory.GetFiles(PackagesConfigFileName, SearchOption.AllDirectories);

            foreach (var assemblyInfoFile in assemblyInfoFiles)
            {
                var deleteFolder = assemblyInfoFile.Directory.GetFiles().Length == 1;
                if (deleteFolder)
                {
                    assemblyInfoFile.Directory.Delete(true);
                }
                else
                {
                    assemblyInfoFile.Delete();
                }
            }

            foreach (var packageFile in packageFiles)
            {
                packageFile.Delete();
            }
        }

        public ImmutableList<ProjectModel> CleanTransitiveReferences(ImmutableList<ProjectModel> projectModels) => CleanTransitiveReferencesInternal(projectModels).ToImmutableList();

        private IEnumerable<ProjectModel> CleanTransitiveReferencesInternal(IEnumerable<ProjectModel> projectModels)
        {
            // TODO: think about building a dependency tree.
            foreach (var model in projectModels)
            {
                var ascendants = GetProjectAscendants(model, projectModels).ToList();
                if (!ascendants.Any())
                {
                    yield return model;
                    continue;
                }

                var ascendantsPackages = ascendants.SelectMany(a => a.PackageDependencies).Distinct();
                var ascendantsProjectReferences = ascendants.SelectMany(a => a.ProjectReferences).Distinct();
                var cleanedPackages = model.PackageDependencies.Except(ascendantsPackages);
                var cleanedReferences = model.ProjectReferences.Except(ascendantsProjectReferences);

                yield return model.WithPackageDependencies(cleanedPackages).WithProjectReferences(cleanedReferences);
            }
        }

        private IEnumerable<ProjectModel> GetProjectAscendants(ProjectModel projectModel, IEnumerable<ProjectModel> allProjectModels)
        {
            foreach (var other in allProjectModels.Where(m => m.ProjectName != projectModel.ProjectName))
            {
                if (projectModel.ProjectReferences.Any(r => r.RelativePath.Contains(other.ProjectName)))
                {
                    yield return other;
                }
            }
        }
    }
}
