namespace ProjectUpgrade.Models
{
    public class ProjectDependencyModel
    {
        public string PackageId { get; }
        public string Version { get; }

        public ProjectDependencyModel(string packageId, string version)
        {
            PackageId = packageId;
            Version = version;
        }
    }
}
