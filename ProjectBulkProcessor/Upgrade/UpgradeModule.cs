using System.IO.Abstractions;
using Autofac;
using ProjectBulkProcessor.Upgrade.Interfaces;
using ProjectBulkProcessor.Upgrade.Processors;

namespace ProjectBulkProcessor.Upgrade
{
    public class UpgradeModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<OptionsParser>().As<IOptionsParser>().InstancePerDependency();
            builder.RegisterType<FileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
            builder.RegisterType<UpgradeOrchestrator>().AsSelf().InstancePerDependency();
        }
    }
}
