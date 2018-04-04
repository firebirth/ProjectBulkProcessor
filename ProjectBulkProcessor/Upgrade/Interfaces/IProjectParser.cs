using System.IO.Abstractions;
using ProjectBulkProcessor.Upgrade.Models;

namespace ProjectBulkProcessor.Upgrade.Interfaces
{
    public interface IProjectParser
    {
        ProjectModel ParseProject(FileInfoBase projectFile);
    }
}
