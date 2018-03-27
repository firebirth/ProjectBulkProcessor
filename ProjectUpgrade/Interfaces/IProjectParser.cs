using System.IO.Abstractions;
using ProjectUpgrade.Models;

namespace ProjectUpgrade.Interfaces
{
    public interface IProjectParser
    {
        ProjectModel ParseProject(FileInfoBase projectFile);
    }
}
