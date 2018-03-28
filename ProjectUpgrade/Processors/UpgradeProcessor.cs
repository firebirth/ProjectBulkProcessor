using System.IO;
using System.IO.Abstractions;
using ProjectUpgrade.Configration;
using ProjectUpgrade.Extensions;
using ProjectUpgrade.Interfaces;

namespace ProjectUpgrade.Processors
{
    public class UpgradeProcessor
    {
        private readonly IFileSystem _fileSystem;
        private readonly IProjectScanner _projectScanner;

        public UpgradeProcessor(IFileSystem fileSystem, IProjectScanner projectScanner)
        {
            _fileSystem = fileSystem;
            _projectScanner = projectScanner;
        }

        public int ProcessProjects(UpgradeParameters parameters)
        {
            var models = _projectScanner.ScanForProjects(parameters.RootDirectory);

            foreach (var projectModel in models)
            {
                var projectBuilder = new ProjectBuilder().AddProjectReferences(projectModel.ProjectReferences)
                                                         .AddPackageDependencies(projectModel.PackageDependencies)
                                                         .SetProjectType(projectModel.IsExecutable);

                using (var fs = projectModel.ProjectFile.OpenWrite())
                {
                    projectBuilder.Build().Save(fs);
                }
            }

            DeleteDeprecatedFiles(parameters.RootDirectory);

            return 1;
        }

        private void DeleteDeprecatedFiles(string rootFolder)
        {
            var rootDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(rootFolder);
            var assemblyInfoFiles = rootDirectory.GetFiles("AssemblyInfo.cs", SearchOption.AllDirectories);
            var packageFiles = rootDirectory.GetFiles("packages.config", SearchOption.AllDirectories);

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
    }
}
