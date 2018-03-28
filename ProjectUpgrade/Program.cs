using CommandLine;
using ProjectUpgrade.Configration;
using ProjectUpgrade.Processors;

namespace ProjectUpgrade
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<UpgradeParameters, EnrichParameters>(args)
                  .MapResult((UpgradeParameters upgrade) => UpgradeProjects(upgrade),
                             (EnrichParameters enrich) => EnrichProjects(enrich),
                             errors => -1);
        }

        private static int EnrichProjects(EnrichParameters enrich)
        {
            throw new System.NotImplementedException();
        }

        private static int UpgradeProjects(UpgradeParameters upgrade)
        {
            throw new System.NotImplementedException();
        }
    }
}
