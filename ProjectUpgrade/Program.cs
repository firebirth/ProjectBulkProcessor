using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ProjectUpgrade
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            foreach (var projectFile in Directory.GetFiles(configuration["rootPath"], "*.csproj", SearchOption.AllDirectories)
                                                 .Select(p => new FileInfo(p)))
            {
                var newProject = new ProjectBuilder(projectFile, configuration)
                    .GenerateCommonSection()
                    .GenerateDependenciesSection()
                    .GenerateProjectReferenceSection()
                    .Build();

                using (var fs = new FileStream(projectFile.FullName, FileMode.Truncate))
                {
                    newProject.Save(fs);
                }
            }

            DeleteDeprecatedFiles(configuration["rootPath"]);
        }

        private static void DeleteDeprecatedFiles(string rootFolder)
        {
            var assemblyInfoFiles = Directory
                .GetFiles(rootFolder, "AssemblyInfo.cs", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f));
            var packageFiles = Directory.GetFiles(rootFolder, "packages.config", SearchOption.AllDirectories)
                                        .Select(f => new FileInfo(f));

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
