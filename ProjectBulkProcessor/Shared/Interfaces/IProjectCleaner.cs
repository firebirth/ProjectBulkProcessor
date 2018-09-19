using ProjectBulkProcessor.Shared.Models;
using System.Collections.Immutable;

namespace ProjectBulkProcessor.Shared.Interfaces
{
    public interface IProjectCleaner
    {
        void DeleteDeprecatedFiles(string rootFolder);

        ImmutableList<ProjectModel> CleanTransitiveReferences(ImmutableList<ProjectModel> projectModels);
    }
}
