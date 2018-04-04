using System.Collections.Immutable;
using System.Linq;
using ProjectBulkProcessor.Configration;
using ProjectBulkProcessor.Upgrade.Extensions;
using ProjectBulkProcessor.Upgrade.Interfaces;
using ProjectBulkProcessor.Upgrade.Processors;

namespace ProjectBulkProcessor.Upgrade
{
    public class UpgradeOrchestrator
    {
        private readonly IProjectScanner _projectScanner;
        private readonly IProjectCleaner _projectCleaner;

        public UpgradeOrchestrator(IProjectScanner projectScanner, IProjectCleaner projectCleaner)
        {
            _projectScanner = projectScanner;
            _projectCleaner = projectCleaner;
        }

        public void ProcessProjects(UpgradeParameters parameters)
        {
            var models = _projectScanner.ScanForProjects(parameters.RootDirectory).ToImmutableList();

            foreach (var projectModel in models)
            {
                var project = ProjectBuilder.CreateProject()
                                            .SetProjectOptions(projectModel.Options)
                                            .AddProjectReferences(projectModel.ProjectReferences)
                                            .AddPackageDependencies(projectModel.PackageDependencies)
                                            .Build();

                using (var fs = projectModel.ProjectFile.OpenWrite())
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
