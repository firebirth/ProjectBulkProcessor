using System.Collections.Immutable;
using System.IO.Abstractions;

namespace ProjectBulkProcessor.Shared.Models
{
    public class ProjectModel
    {
        public string ProjectName => ProjectFile.Name;
        public FileInfoBase ProjectFile { get; }
        public IImmutableList<ProjectReferenceModel> ProjectReferences { get; }
        public IImmutableList<PackageDependencyModel> PackageDependencies { get; }

        public ProjectModel(FileInfoBase projectFile,
                            IImmutableList<ProjectReferenceModel> projectReferences,
                            IImmutableList<PackageDependencyModel> packageDependencies)
        {
            ProjectFile = projectFile;
            ProjectReferences = projectReferences;
            PackageDependencies = packageDependencies;
        }
    }
}
