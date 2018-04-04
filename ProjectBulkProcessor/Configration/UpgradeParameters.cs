using CommandLine;

namespace ProjectUpgrade.Configration
{
    [Verb("upgrade", HelpText = "Upgrades project to use VS2017 structure")]
    public class UpgradeParameters
    {
        public UpgradeParameters(string rootPath)
        {
            RootDirectory = rootPath;
        }

        [Option('r', "rootPath", Required = true, HelpText = "Root path to scan for project files")]
        public string RootDirectory { get; }
    }
}
