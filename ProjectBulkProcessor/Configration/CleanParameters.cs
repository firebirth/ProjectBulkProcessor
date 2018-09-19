using CommandLine;

namespace ProjectBulkProcessor.Configration
{
    [Verb("clean", HelpText = "Cleans VS2017 projects from transitive references")]
    public class CleanParameters
    {
        public CleanParameters()
        {
        }

        public CleanParameters(string rootPath)
        {
            RootPath = rootPath;
        }

        [Option('r', "rootPath", Required = true, HelpText = "Root path to scan for project files")]
        public string RootPath { get; }
    }
}
