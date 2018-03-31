namespace ProjectUpgrade.Upgrade.Interfaces
{
    public interface IProjectCleaner
    {
        void DeleteDeprecatedFiles(string rootFolder);
    }
}
