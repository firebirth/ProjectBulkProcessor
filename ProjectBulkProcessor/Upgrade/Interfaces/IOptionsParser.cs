using System.IO.Abstractions;
using ProjectBulkProcessor.Upgrade.Models;

namespace ProjectBulkProcessor.Upgrade.Interfaces
{
    public interface IOptionsParser
    {
        OptionsModel ParseProjectOptions(FileInfoBase projectFile);
    }
}
