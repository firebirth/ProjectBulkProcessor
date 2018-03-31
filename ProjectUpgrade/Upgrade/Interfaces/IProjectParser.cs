using System.IO.Abstractions;
using ProjectUpgrade.Models;

namespace ProjectUpgrade.Upgrade.Interfaces
{
    public interface IProjectParser
    {
        ProjectModel ParseProject(FileInfoBase projectFile);
    }
}
