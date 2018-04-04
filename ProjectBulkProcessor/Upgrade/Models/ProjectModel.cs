using System.Collections.Immutable;
using System.IO.Abstractions;

namespace ProjectUpgrade.Upgrade.Models
{
    public class ProjectModel
    {
        public FileInfoBase ProjectFile { get; }
        public IImmutableList<ProjectReferenceModel> ProjectReferences { get; }
        public IImmutableList<PackageDependencyModel> PackageDependencies { get; }
        public OptionsModel Options { get; }

        public ProjectModel(FileInfoBase projectFile,
                            IImmutableList<ProjectReferenceModel> projectReferences,
                            IImmutableList<PackageDependencyModel> packageDependencies,
                            OptionsModel options)
        {
            ProjectFile = projectFile;
            ProjectReferences = projectReferences;
            PackageDependencies = packageDependencies;
            Options = options;
        }
    }
}
