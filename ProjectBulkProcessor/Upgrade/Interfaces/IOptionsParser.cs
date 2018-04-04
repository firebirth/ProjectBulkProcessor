using System.IO.Abstractions;
using ProjectUpgrade.Upgrade.Models;

namespace ProjectUpgrade.Upgrade.Interfaces
{
    public interface IOptionsParser
    {
        OptionsModel ParseProjectOptions(FileInfoBase projectFile);
    }
}
