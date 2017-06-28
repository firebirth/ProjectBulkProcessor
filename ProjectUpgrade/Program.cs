using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ProjectUpgrade.Models;

namespace ProjectUpgrade
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            foreach (var projectModel in Directory
                .GetFiles(configuration["rootPath"], "*.csproj", SearchOption.AllDirectories)
                .Select(ProjectModel.Parse))
            {
                var newProject = new ProjectBuilder(projectModel, configuration)
                    .GenerateCommonSection()
                    .GenerateDependenciesSection()
                    .GenerateProjectReferenceSection()
                    .Build();

                using (var fs = new FileStream(projectModel.ProjectFile.FullName, FileMode.Truncate))
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
