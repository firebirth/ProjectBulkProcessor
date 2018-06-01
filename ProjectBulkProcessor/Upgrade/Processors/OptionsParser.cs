using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProjectBulkProcessor.Upgrade.Interfaces;
using ProjectBulkProcessor.Upgrade.Models;

namespace ProjectBulkProcessor.Upgrade.Processors
{
    public class OptionsParser : IOptionsParser
    {
        private static readonly PropertyInfo[] OptionsModelProperties;
        public static readonly ImmutableDictionary<string, string> PropertyNameAssemblyInfoAttributeMap;
        public static readonly ImmutableDictionary<string, string> AssemblyInfoAttributePropertyNameMap;

        static OptionsParser()
        {
            AssemblyInfoAttributePropertyNameMap = new Dictionary<string, string>
            {
                {nameof(AssemblyCompanyAttribute), nameof(OptionsModel.Company)},
                {nameof(AssemblyCopyrightAttribute), nameof(OptionsModel.Copyright)},
                {nameof(AssemblyDescriptionAttribute), nameof(OptionsModel.Description)},
                {nameof(AssemblyProductAttribute), nameof(OptionsModel.Product)},
                {nameof(AssemblyVersionAttribute), nameof(OptionsModel.Version)}
            }.ToImmutableDictionary();

            PropertyNameAssemblyInfoAttributeMap =
                AssemblyInfoAttributePropertyNameMap.ToImmutableDictionary(kvp => kvp.Value, kvp => kvp.Key);

            OptionsModelProperties = typeof(OptionsModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public OptionsModel ParseProjectOptions(FileInfoBase projectFile)
        {
            var options = new OptionsModel();

            SetProjectOptions(projectFile, options);
            SetAssemblyInfoOptions(projectFile.Directory, options);

            return options;
        }

        private void SetAssemblyInfoOptions(DirectoryInfoBase projectDirectory, OptionsModel options)
        {
            var assemblyInfoFile = projectDirectory.GetFiles("AssemblyInfo.cs", SearchOption.AllDirectories).SingleOrDefault();
            if (assemblyInfoFile == null)
            {
                return;
            }

            SyntaxTree tree;
            using (var fs = assemblyInfoFile.OpenText())
            {
                var content = fs.ReadToEnd();
                tree = CSharpSyntaxTree.ParseText(content);
            }

            var root = (CompilationUnitSyntax)tree.GetRoot();
            var attributeLists = root.AttributeLists;

            foreach (var attributeList in attributeLists)
            {
                foreach (var attributeName in AssemblyInfoAttributePropertyNameMap.Keys)
                {
                    var attribute = attributeList.Attributes.FirstOrDefault(a => (a.Name as IdentifierNameSyntax)?.Identifier.Text == attributeName);
                    if (attribute == null)
                    {
                        continue;
                    }

                    var property = OptionsModelProperties.Single(p => p.Name == AssemblyInfoAttributePropertyNameMap[attributeName]);
                    var attributeArguments = attribute.ArgumentList.Arguments.Select(a => a.ToFullString().Trim('"'));

                    property.SetValue(options, string.Join(",", attributeArguments));
                }
            }
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
