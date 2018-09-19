using ProjectBulkProcessor.Upgrade.Models;
using System.Collections.Immutable;

namespace ProjectBulkProcessor.Upgrade.Interfaces
{
    public interface IProjectCleaner
    {
        void DeleteDeprecatedFiles(string rootFolder);

        IImmutableList<ProjectModel> CleanTransitiveReferences(IImmutableList<ProjectModel> projectModels);
    }
}
