using System.IO.Abstractions;
using Autofac;
using ProjectUpgrade.Upgrade.Interfaces;
using ProjectUpgrade.Upgrade.Processors;

namespace ProjectUpgrade.Upgrade
{
    public class UpgradeModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ProjectCleaner>().As<IProjectCleaner>().InstancePerDependency();
            builder.RegisterType<ProjectParser>().As<IProjectParser>().InstancePerDependency();
            builder.RegisterType<ProjectScanner>().As<IProjectScanner>().InstancePerDependency();
            builder.RegisterType<OptionsParser>().As<IOptionsParser>().InstancePerDependency();
            builder.RegisterType<FileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
            builder.RegisterType<UpgradeOrchestrator>().AsSelf().InstancePerDependency();
        }
    }
}
