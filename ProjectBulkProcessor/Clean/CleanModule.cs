using Autofac;

namespace ProjectBulkProcessor.Clean
{
    public class CleanModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CleanOrchestrator>()
                .As<ICleanOrchestrator>();
        }
    }
}
