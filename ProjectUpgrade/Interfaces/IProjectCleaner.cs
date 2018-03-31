namespace ProjectUpgrade.Interfaces
{
    public interface IProjectCleaner
    {
        void DeleteDeprecatedFiles(string rootFolder);
    }
}
