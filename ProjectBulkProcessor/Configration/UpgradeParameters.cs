using System.Collections.Generic;
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace ProjectBulkProcessor.Configration
{
    [Verb("upgrade", HelpText = "Upgrades project to use VS2017 structure")]
    public class UpgradeParameters
    {
        public UpgradeParameters()
        {
        }

        public UpgradeParameters(string rootPath)
        {
            RootDirectory = rootPath;
        }

        [Option('r', "rootPath", Required = true, HelpText = "Root path to scan for project files")]
        public string RootDirectory { get; set; }

        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("example", new UpgradeParameters(Directory.GetCurrentDirectory()));
            }
        }
    }
}
