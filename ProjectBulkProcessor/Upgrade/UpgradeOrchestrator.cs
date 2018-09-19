using System.Collections.Immutable;
using System.Linq;
using ProjectBulkProcessor.Configration;
using ProjectBulkProcessor.Shared.Interfaces;
using ProjectBulkProcessor.Upgrade.Extensions;
using ProjectBulkProcessor.Upgrade.Interfaces;
using ProjectBulkProcessor.Upgrade.Processors;

namespace ProjectBulkProcessor.Upgrade
{
    public class UpgradeOrchestrator
    {
        private readonly IProjectScanner _projectScanner;
        private readonly IProjectCleaner _projectCleaner;
        private readonly IOptionsParser _optionsParser;

        public UpgradeOrchestrator(IProjectScanner projectScanner, IProjectCleaner projectCleaner, IOptionsParser optionsParser)
        {
            _projectScanner = projectScanner;
            _projectCleaner = projectCleaner;
            _optionsParser = optionsParser;
        }

        public void ProcessProjects(UpgradeParameters parameters)
        {
            var models = _projectScanner.ScanForProjects(parameters.RootDirectory).ToImmutableList();

            foreach (var projectModel in models)
            {
                var options = _optionsParser.ParseProjectOptions(projectModel.ProjectFile);
                var project = ProjectBuilder.CreateProject()
                                            .SetProjectOptions(options)
                                            .AddProjectReferences(projectModel.ProjectReferences)
                                            .AddPackageDependencies(projectModel.PackageDependencies)
                                            .Build();

                using (var fs = projectModel.ProjectFile.CreateText())
                {
                    project.Save(fs);
                }
            }

            if (models.Any())
            {
                _projectCleaner.DeleteDeprecatedFiles(parameters.RootDirectory);
            }
        }
    }
}
