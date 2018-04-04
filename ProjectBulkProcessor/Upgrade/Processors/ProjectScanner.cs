using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using ProjectBulkProcessor.Upgrade.Interfaces;
using ProjectBulkProcessor.Upgrade.Models;

namespace ProjectBulkProcessor.Upgrade.Processors
{
    public class ProjectScanner : IProjectScanner
    {
        private readonly IProjectParser _projectParser;
        private readonly IFileSystem _fileSystem;

        public ProjectScanner(IProjectParser projectParser, IFileSystem fileSystem)
        {
            _projectParser = projectParser;
            _fileSystem = fileSystem;
        }

        public IEnumerable<ProjectModel> ScanForProjects(string rootFolder)
        {
            var rootDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(rootFolder);
            if (!rootDirectory.Exists)
            {
                throw new DirectoryNotFoundException($"Directory {rootFolder} doesn't exist.");
            }

            var projectFiles = rootDirectory.GetFiles("*.csproj", SearchOption.AllDirectories);
            if (!projectFiles.Any())
            {
                yield break;
            }

            foreach (var projectFile in projectFiles)
            {
                yield return _projectParser.ParseProject(projectFile);
            }
        }
    }
}
