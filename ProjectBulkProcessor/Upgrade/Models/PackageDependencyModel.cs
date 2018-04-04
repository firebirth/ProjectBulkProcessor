using System;

namespace ProjectUpgrade.Upgrade.Models
{
    public class PackageDependencyModel : IEquatable<PackageDependencyModel>
    {
        public string PackageId { get; }
        public string Version { get; }

        public PackageDependencyModel(string packageId, string version)
        {
            PackageId = packageId;
            Version = version;
        }

        public bool Equals(PackageDependencyModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(PackageId, other.PackageId, StringComparison.OrdinalIgnoreCase) 
                   && string.Equals(Version, other.Version, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PackageDependencyModel) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((PackageId != null ? PackageId.GetHashCode() : 0) * 397) ^ (Version != null ? Version.GetHashCode() : 0);
            }
        }
    }
}
