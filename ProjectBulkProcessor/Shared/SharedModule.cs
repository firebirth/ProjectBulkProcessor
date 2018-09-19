using System.IO.Abstractions;
using Autofac;
using ProjectBulkProcessor.Shared.Interfaces;
using ProjectBulkProcessor.Shared.Processors;

namespace ProjectBulkProcessor.Shared
{
    public class SharedModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ProjectCleaner>().As<IProjectCleaner>().InstancePerDependency();
            builder.RegisterType<ProjectParser>().As<IProjectParser>().InstancePerDependency();
            builder.RegisterType<ProjectScanner>().As<IProjectScanner>().InstancePerDependency();
            builder.RegisterType<FileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
        }
    }
}
