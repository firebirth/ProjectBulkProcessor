using ProjectBulkProcessor.Shared.Models;
using System.Collections.Immutable;

namespace ProjectBulkProcessor.Shared.Interfaces
{
    public interface IProjectCleaner
    {
        void DeleteDeprecatedFiles(string rootFolder);

        IImmutableList<ProjectModel> CleanTransitiveReferences(IImmutableList<ProjectModel> projectModels);
    }
}
