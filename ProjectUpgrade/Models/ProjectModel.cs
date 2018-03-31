using System.Collections.Immutable;
using System.IO.Abstractions;

namespace ProjectUpgrade.Models
{
    public class ProjectModel
    {
        public FileInfoBase ProjectFile { get; }
        public IImmutableList<ProjectReferenceModel> ProjectReferences { get; }
        public IImmutableList<PackageDependencyModel> PackageDependencies { get; }
        public bool IsExecutable { get; }

        public ProjectModel(
            FileInfoBase projectFile,
            IImmutableList<ProjectReferenceModel> projectReferences,
            IImmutableList<PackageDependencyModel> packageDependencies,
            bool isExecutable)
        {
            ProjectFile = projectFile;
            ProjectReferences = projectReferences;
            PackageDependencies = packageDependencies;
            IsExecutable = isExecutable;
        }
    }
}
