namespace ProjectBulkProcessor.Clean
{
    public interface ICleanOrchestrator
    {
        void CleanProjects(string rootFolder);
    }
}