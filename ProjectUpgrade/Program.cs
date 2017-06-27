using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
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
            var propertyFolders = Directory
                .GetFiles(rootFolder, "AssemblyInfo.cs", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f).Directory);
            var packageFiles = Directory.GetFiles(rootFolder, "packages.config", SearchOption.AllDirectories)
                                        .Select(f => new FileInfo(f));

            foreach (var itemToDelete in propertyFolders)
            {
                itemToDelete.Delete(true);
            }

            foreach (var packageFile in packageFiles)
            {
                packageFile.Delete();
            }
        }
    }
}
