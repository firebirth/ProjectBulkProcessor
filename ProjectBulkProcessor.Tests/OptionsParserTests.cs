using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using ProjectBulkProcessor.Upgrade.Processors;
using ProjectBulkProcessor.Tests.Assertions;
using ProjectBulkProcessor.Upgrade.Models;
using Xunit;


namespace ProjectBulkProcessor.Tests
{
    public class OptionsParserTests
    {
        private const string ProjectFileName = "path";
        private readonly OptionsParser _sut = new OptionsParser();
        private readonly MockFileSystem _fileSystem = new MockFileSystem();

        public OptionsParserTests()
        {
            _fileSystem.AddFile(ProjectFileName, new MockFileData("<root><TargetFrameworkVersion>testVersion</TargetFrameworkVersion><OutputType>Exe</OutputType></root>"));
        }

        [Theory]
        [InlineData(nameof(OptionsModel.TargetFramework), "testVersion")]
        [InlineData(nameof(OptionsModel.IsExecutable), "True")]
        public void ShouldParseProjectOptions(string propertyName, string expectedValue)
        {
            var fileInfo = GetFileInfo();

            var result = _sut.ParseProjectOptions(fileInfo);

            result.Should().HaveProperty(propertyName)
                  .Which.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(nameof(OptionsModel.Description), "testDescription")]
        public void ShouldParseAssemblyInfoOptions(string propertyName, string expectedValue)
        {
            _fileSystem.AddFile("AssemblyInfo.cs", new MockFileData(GetAssemblyInfoAttribute(propertyName, expectedValue)));

            var fileInfo = GetFileInfo();
            var result = _sut.ParseProjectOptions(fileInfo);

            result.Should().HaveProperty(propertyName)
                  .Which.Should().Be(expectedValue);
        }

        private static string GetAssemblyInfoAttribute(string propertyName, string attributeValue)
        {
            var attributeName = OptionsParser.PropertyNameAssemblyInfoAttributeMap[propertyName];
            return $"[assembly: {attributeName}(\"{attributeValue}\")]";
        }

        private FileInfoBase GetFileInfo()
        {
            var fileName = _fileSystem.AllFiles.Single(f => f.Contains(ProjectFileName));
            return _fileSystem.FileInfo.FromFileName(fileName);
        }
    }
}
