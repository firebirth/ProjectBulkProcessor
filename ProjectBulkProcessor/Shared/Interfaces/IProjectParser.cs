using System.IO.Abstractions;
using ProjectBulkProcessor.Shared.Models;

namespace ProjectBulkProcessor.Shared.Interfaces
{
    public interface IProjectParser
    {
        ProjectModel ParseProject(FileInfoBase projectFile);
    }
}
