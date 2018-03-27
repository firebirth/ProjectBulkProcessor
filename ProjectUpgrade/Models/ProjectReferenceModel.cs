using System;

namespace ProjectUpgrade.Models
{
    public class ProjectReferenceModel : IEquatable<ProjectReferenceModel>
    {
        public string RelativePath { get; }

        public ProjectReferenceModel(string relativePath)
        {
            RelativePath = relativePath;
        }

        public bool Equals(ProjectReferenceModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(RelativePath, other.RelativePath, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProjectReferenceModel) obj);
        }

        public override int GetHashCode()
        {
            return (RelativePath != null ? RelativePath.GetHashCode() : 0);
        }
    }
}
