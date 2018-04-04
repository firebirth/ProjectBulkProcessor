using System.IO;
using System.IO.Abstractions;
using ProjectUpgrade.Upgrade.Interfaces;

namespace ProjectUpgrade.Upgrade.Processors
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
    }
}
