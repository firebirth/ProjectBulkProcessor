using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProjectUpgrade.Upgrade.Interfaces;
using ProjectUpgrade.Upgrade.Models;

namespace ProjectUpgrade.Upgrade.Processors
{
    public class OptionsParser : IOptionsParser
    {
        public OptionsModel ParseProjectOptions(FileInfoBase projectFile)
        {
            var options = new OptionsModel();

            SetProjectOptions(projectFile, options);
            SetAssemblyInfoOptions(projectFile.Directory, options);

            return options;
        }

        private void SetAssemblyInfoOptions(DirectoryInfoBase projectDirectory, OptionsModel options)
        {
            var assemblyInfoFile = projectDirectory.GetFiles("AssemblyInfo.cs", SearchOption.AllDirectories).Single();
            SyntaxTree tree;
            using (var fs = assemblyInfoFile.OpenText())
            {
                var content = fs.ReadToEnd();
                tree = CSharpSyntaxTree.ParseText(content);
            }

            var root = (CompilationUnitSyntax)tree.GetRoot();
            var attributes = root.AttributeLists.GetEnumerator();

        }

        private void SetProjectOptions(FileInfoBase fileInfo, OptionsModel options)
        {
            XDocument doc;
            using (var fs = fileInfo.OpenRead())
            {
                doc = XDocument.Load(fs);
            }

            options.TargetFramework = doc.Descendants("TargetFrameworkVersion")
                                         .Single().Value;
            options.IsExecutable = doc.Descendants("OutputType")
                                      .Any(x => x.Value == "Exe");
        }
    }
}
