using System;
using Autofac;
using CommandLine;
using ProjectBulkProcessor.Clean;
using ProjectBulkProcessor.Configration;
using ProjectBulkProcessor.Extensions;
using ProjectBulkProcessor.Shared;
using ProjectBulkProcessor.Upgrade;

namespace ProjectBulkProcessor
{
    public class Program
    {
        private static readonly IContainer Container = SetupContainer();

        public static void Main(string[] args)
        {
            Container.AssertConfigurationIsValid();

            Parser.Default.ParseArguments<UpgradeParameters, EnrichParameters, CleanParameters>(args)
                  .MapResult((UpgradeParameters upgrade) => UpgradeProjects(upgrade),
                             (EnrichParameters enrich) => EnrichProjects(enrich),
                             (CleanParameters clean) => CleanProjects(clean),
                              errors => -1);
        }

        private static int CleanProjects(CleanParameters clean)
        {
            try
            {
                var orchestrator = Container.Resolve<ICleanOrchestrator>();
                orchestrator.CleanProjects(clean.RootPath);
                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return -1;
            }
        }

        private static int EnrichProjects(EnrichParameters enrich)
        {
            throw new NotImplementedException();
        }

        private static int UpgradeProjects(UpgradeParameters upgrade)
        {
            try
            {
                var orchestrator = Container.Resolve<UpgradeOrchestrator>();
                orchestrator.ProcessProjects(upgrade);
                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return -1;
            }
        }

        private static IContainer SetupContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<SharedModule>();
            builder.RegisterModule<UpgradeModule>();
            builder.RegisterModule<CleanModule>();

            return builder.Build();
        }
    }
}
