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
        }
    }
}
