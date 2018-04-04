namespace ProjectBulkProcessor.Upgrade.Interfaces
{
    public interface IProjectCleaner
    {
        void DeleteDeprecatedFiles(string rootFolder);
    }
}
