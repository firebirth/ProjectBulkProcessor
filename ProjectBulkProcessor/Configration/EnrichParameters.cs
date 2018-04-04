using CommandLine;

namespace ProjectBulkProcessor.Configration
{
    [Verb("enrich", HelpText = "Enriches VS2017 projects with given info")]
    public class EnrichParameters
    {
        public EnrichParameters(string rootPath, string targetFramework, string copyright, string company, string authors, string description, string packageLicenseUrl, string packageProjectUrl, string packageIconUrl, string repositoryUrl, string repositoryType, string packageTags, string packageReleaseNotes, string packageId, string version, string product)
        {
            RootPath = rootPath;
            // all of below go to NEW PropertyGroup as nodes with inner text, with node names starting from capital letter
            TargetFramework = targetFramework;
            Copyright = copyright;
            Company = company;
            Authors = authors;
            Description = description;
            PackageLicenseUrl = packageLicenseUrl;
            PackageProjectUrl = packageProjectUrl;
            PackageIconUrl = packageIconUrl;
            RepositoryUrl = repositoryUrl;
            RepositoryType = repositoryType;
            PackageTags = packageTags;
            PackageReleaseNotes = packageReleaseNotes;
            PackageId = packageId;
            Version = version;
            Product = product;
        }

        [Option('r', "rootPath", Required = true, HelpText = "Root path to scan for project files")]
        public string RootPath { get; }

        [Option("targetFramework", Required = false)]
        public string TargetFramework { get; }

        [Option("copyright", Required = false)]
        public string Copyright { get; }

        [Option("company", Required = false)]
        public string Company { get; }

        [Option("authors", Required = false)]
        public string Authors { get; }

        [Option("description", Required = false)]
        public string Description { get; }

        [Option("packageLicenseUrl", Required = false)]
        public string PackageLicenseUrl { get; }

        [Option("packageProjectUrl", Required = false)]
        public string PackageProjectUrl { get; }

        [Option("packageIconUrl", Required = false)]
        public string PackageIconUrl { get; }

        [Option("repositoryUrl", Required = false)]
        public string RepositoryUrl { get; }

        [Option("repositoryType", Required = false)]
        public string RepositoryType { get; }

        [Option("packageTags", Required = false)]
        public string PackageTags { get; }

        [Option("packageReleaseNotes", Required = false)]
        public string PackageReleaseNotes { get; }

        [Option("packageId", Required = false)]
        public string PackageId { get; }

        [Option("version", Required = false)]
        public string Version { get; }

        [Option("product", Required = false)]
        public string Product { get; }
    }
}
