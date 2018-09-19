using System;
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
using ProjectBulkProcessor.Extensions;
using ProjectBulkProcessor.Upgrade.Interfaces;
using ProjectBulkProcessor.Upgrade.Models;

namespace ProjectBulkProcessor.Upgrade.Processors
{
    public class OptionsParser : IOptionsParser
    {
        private static readonly PropertyInfo[] OptionsModelProperties;
        public static readonly ImmutableDictionary<string, string> OldNewFrameworkAliasMap;
        public static readonly ImmutableDictionary<string, string> PropertyNameAssemblyInfoAttributeMap;
        public static readonly ImmutableDictionary<string, string> AssemblyInfoAttributePropertyNameMap;

        static OptionsParser()
        {
            AssemblyInfoAttributePropertyNameMap = new Dictionary<string, string>
            {
                [nameof(AssemblyCompanyAttribute)] = nameof(OptionsModel.Company),
                [nameof(AssemblyCopyrightAttribute)] = nameof(OptionsModel.Copyright),
                [nameof(AssemblyDescriptionAttribute)] = nameof(OptionsModel.Description),
                [nameof(AssemblyProductAttribute)] = nameof(OptionsModel.Product),
                [nameof(AssemblyVersionAttribute)] = nameof(OptionsModel.Version)
            }.ToImmutableDictionary();

            PropertyNameAssemblyInfoAttributeMap = AssemblyInfoAttributePropertyNameMap.ToImmutableDictionary(kvp => kvp.Value, kvp => kvp.Key);

            OldNewFrameworkAliasMap = new Dictionary<string, string>
            {
                // TODO: add other frameworks
                ["v4.6.2"] = "net462",
                ["v4.6.1"] = "net461",
                ["v4.5.2"] = "net452",
                ["v4.5"] = "net45",
                ["v4.0"] = "net40",
            }.ToImmutableDictionary();

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
                foreach (var optionAttributeName in AssemblyInfoAttributePropertyNameMap.Keys)
                {
                    var shortAttributeName = optionAttributeName.Replace(nameof(Attribute), string.Empty);
                    var attribute = attributeList.Attributes.FirstOrDefault(a =>
                    {
                        var attributeName = (a.Name as IdentifierNameSyntax)?.Identifier.Text;
                        return string.Equals(optionAttributeName, attributeName, StringComparison.Ordinal) || string.Equals(attributeName, shortAttributeName, StringComparison.Ordinal);
                    });
                    if (attribute == null)
                    {
                        continue;
                    }

                    var property = OptionsModelProperties.Single(p => p.Name == AssemblyInfoAttributePropertyNameMap[optionAttributeName]);
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

            var oldFramework = doc.GetProjectElementByName("TargetFrameworkVersion").Value;
            if (!OldNewFrameworkAliasMap.TryGetValue(oldFramework, out var newFramework))
            {
                throw new NotSupportedException($"Framework {oldFramework} is not supported.");
            }

            options.TargetFramework = newFramework;
            options.IsExecutable = doc.GetProjectElementByName("OutputType").Value.Contains("Exe");
        }
    }
}
