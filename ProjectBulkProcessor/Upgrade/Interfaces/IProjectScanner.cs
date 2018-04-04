using System.Collections.Generic;
using ProjectBulkProcessor.Upgrade.Models;

namespace ProjectBulkProcessor.Upgrade.Interfaces
{
    public interface IProjectScanner
    {
        IEnumerable<ProjectModel> ScanForProjects(string rootFolder);
    }
}
