using System.Collections.Generic;
using ProjectUpgrade.Upgrade.Models;

namespace ProjectUpgrade.Upgrade.Interfaces
{
    public interface IProjectScanner
    {
        IEnumerable<ProjectModel> ScanForProjects(string rootFolder);
    }
}
