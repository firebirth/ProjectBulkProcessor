namespace ProjectUpgrade.Models
{
    public class ProjectReferenceModel
    {
        public string RelativePath { get; }

        public ProjectReferenceModel(string relativePath)
        {
            RelativePath = relativePath;
        }
    }
}
