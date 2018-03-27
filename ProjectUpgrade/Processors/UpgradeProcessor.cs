using System.IO;
using System.IO.Abstractions;
using ProjectUpgrade.Configration;

namespace ProjectUpgrade.Processors
{
    public static class UpgradeProcessor
    {
        public static int ProcessProjects(UpgradeParameters parameters)
        {
            var scanner = new ProjectScanner(null, null);
            var models = scanner.ScanForProjects(parameters.RootDirectory);

            foreach (var projectModel in models)
            {
                var projectBuilder = new ProjectBuilder();

                var groupBuilder = projectBuilder.AddGroup("ItemGroup");
                foreach (var reference in projectModel.ProjectReferences)
                {
                    groupBuilder.WithElement("ProjectReference")
                                .WithAttribute("Include", reference.RelativePath);
                }

                groupBuilder = groupBuilder.AddGroup("ItemGroup");
                foreach (var dependency in projectModel.PackageDependencies)
                {
                    groupBuilder.WithElement("PackageReference")
                                .WithAttribute("Include", dependency.PackageId)
                                .WithAttribute("Version", dependency.Version);
                }

                if (projectModel.IsExecutable)
                {
                    groupBuilder = groupBuilder.AddGroup("PropertyGroup");
                    groupBuilder.WithElement("OutputType")
                                .WithNodeValue("Exe");
                }

                var result = groupBuilder.Build();

                using (var fs = new FileStream(projectModel.ProjectFile.FullName, FileMode.Truncate))
                {
                    result.Save(fs);
                }
            }

            DeleteDeprecatedFiles(parameters.RootDirectory);

            return 1;
        }

        private static void DeleteDeprecatedFiles(string rootFolder)
        {
            var assemblyInfoFiles = rootFolder.GetFiles("AssemblyInfo.cs", SearchOption.AllDirectories);
            var packageFiles = rootFolder.GetFiles("packages.config", SearchOption.AllDirectories);

            foreach (var assemblyInfoFile in assemblyInfoFiles)
            {
                var deleteFolder = assemblyInfoFile.Directory.GetFiles().Length == 1;
                if (deleteFolder)
                {
                    assemblyInfoFile.Directory.Delete(true);
                }
                else
                {
                    assemblyInfoFile.Delete();
                }
            }

            foreach (var packageFile in packageFiles)
            {
                packageFile.Delete();
            }
        }
    }
}
