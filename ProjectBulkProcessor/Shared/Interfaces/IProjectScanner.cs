using System.Collections.Generic;
using ProjectBulkProcessor.Shared.Models;

namespace ProjectBulkProcessor.Shared.Interfaces
{
    public interface IProjectScanner
    {
        IEnumerable<ProjectModel> ScanForProjects(string rootFolder);
    }
}
