using ProjectBulkProcessor.Shared.Interfaces;
using System.Collections.Immutable;

namespace ProjectBulkProcessor.Clean
{
    public class CleanOrchestrator : ICleanOrchestrator
    {
        private readonly IProjectScanner _projectScanner;
        private readonly IProjectCleaner _projectCleaner;

        public CleanOrchestrator(IProjectScanner projectScanner, IProjectCleaner projectCleaner)
        {
            _projectScanner = projectScanner;
            _projectCleaner = projectCleaner;
        }

        public void CleanProjects(string rootFolder)
        {
            var foundProjects = _projectScanner.ScanForProjects(rootFolder).ToImmutableList();
            var cleaned = _projectCleaner.CleanTransitiveReferences(foundProjects);
        }
    }
}
