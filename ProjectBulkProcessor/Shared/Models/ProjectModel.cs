using System.Collections.Generic;
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

        public ProjectModel WithPackageDependencies(IEnumerable<PackageDependencyModel> packageDependencyModels) => new ProjectModel(ProjectFile, ProjectReferences, packageDependencyModels.ToImmutableList());

        public ProjectModel WithProjectReferences(IEnumerable<ProjectReferenceModel> projectReferences) => new ProjectModel(ProjectFile, projectReferences.ToImmutableList(), PackageDependencies);
    }
}
