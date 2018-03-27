using System.Collections.Generic;
using ProjectUpgrade.Models;

namespace ProjectUpgrade.Interfaces
{
    public interface IProjectScanner
    {
        IEnumerable<ProjectModel> ScanForProjects(string rootFolder);
    }
}
