﻿using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using Moq;
using ProjectBulkProcessor.Tests.Assertions;
using ProjectBulkProcessor.Upgrade.Interfaces;
using ProjectBulkProcessor.Upgrade.Models;
using ProjectBulkProcessor.Upgrade.Processors;
using Xunit;

namespace ProjectBulkProcessor.Tests.Upgrade
{
    public class ProjectParserTests
    {
        private const string DirectoryWithPackages = "DirectoryWithPackages";
        private const string DirectoryWithoutPackages = "DirectoryWithoutPackages";
        private const string ProjectWithReferenceAndExe = "ProjectWithReferenceAndExe";
        private const string ProjectWithReference = "ProjectWithOnlyReference";
        private const string ProjectWithExe = "ProjectWithOnlyExe";
        private const string EmptyProject = "EmptyProject";
        private const string MultipleReferenceProject = "MultipleReferenceProject";
        private const string ReferencedProjectPath = "testProjectPath";
        private const string PackageId = "testPackageId";
        private const string PackageVersion = "testVersion";
        private readonly ProjectParser _sut;
        private readonly MockFileSystem _fileSystem;
        private readonly Mock<IOptionsParser> _optionsParserMock;

        public ProjectParserTests()
        {
            _optionsParserMock = new Mock<IOptionsParser>();
            _optionsParserMock.Setup(parser => parser.ParseProjectOptions(It.IsAny<FileInfoBase>())).Returns(new OptionsModel());
            _sut = new ProjectParser(_optionsParserMock.Object);
            _fileSystem = SetFileSystem();
        }

        [Theory]
        [InlineData(DirectoryWithPackages, ProjectWithReferenceAndExe, true, true)]
        [InlineData(DirectoryWithPackages, ProjectWithReference, true, true)]
        [InlineData(DirectoryWithPackages, ProjectWithExe, false, true)]
        [InlineData(DirectoryWithPackages, EmptyProject, false, true)]
        [InlineData(DirectoryWithoutPackages, ProjectWithReferenceAndExe, true, false)]
        [InlineData(DirectoryWithoutPackages, ProjectWithReference, true, false)]
        [InlineData(DirectoryWithoutPackages, ProjectWithExe, false, false)]
        [InlineData(DirectoryWithoutPackages, EmptyProject, false, false)]
        public void ShouldParseProjectWithPackageDependencyAndProjectReferenceAndOfExecutableType(
            string directoryName,
            string projectName,
            bool hasProjectReference,
            bool hasPackageDependency)
        {
            var fileInfo = GetFileInfo(projectName, directoryName);

            var result = _sut.ParseProject(fileInfo);

            result.Should()
                  .WhenShouldHavePackageDependency(hasPackageDependency)
                  .And.WhenShouldHaveProjectReference(hasProjectReference)
                  .Then.HaveLocation(fileInfo.FullName)
                  .And.HaveProjectReference(ReferencedProjectPath)
                  .And.HavePackageDependency(PackageId, PackageVersion);

            _optionsParserMock.Verify(parser => parser.ParseProjectOptions(fileInfo), Times.Once);
        }

        [Fact]
        public void ShouldParseMultipleReferences()
        {
            var fileInfo = GetFileInfo(MultipleReferenceProject, DirectoryWithPackages);

            var result = _sut.ParseProject(fileInfo);

            result.ProjectReferences.Should().HaveCount(2);

            _optionsParserMock.Verify(parser => parser.ParseProjectOptions(fileInfo), Times.Once);
        }

        private FileInfoBase GetFileInfo(string projectName, string directoryName)
        {
            var fileName = _fileSystem.AllFiles.Single(f => f.EndsWith(".csproj") && f.Contains(projectName) && f.Contains(directoryName));
            return _fileSystem.FileInfo.FromFileName(fileName);
        }

        private static MockFileSystem SetFileSystem()
        {
            var mockFileSystem = new MockFileSystem();

            mockFileSystem.AddDirectory(DirectoryWithPackages);
            mockFileSystem.AddDirectory(DirectoryWithoutPackages);

            mockFileSystem.AddFile(DirectoryWithPackages + mockFileSystem.Path.DirectorySeparatorChar + ProjectWithReferenceAndExe + ".csproj",
                                   new MockFileData($"<root xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">{GetTestProjectFileData(true, true)}</root>"));
            mockFileSystem.AddFile(DirectoryWithPackages + mockFileSystem.Path.DirectorySeparatorChar + ProjectWithReference + ".csproj",
                                   new MockFileData($"<root xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">{GetTestProjectFileData(true, false)}</root>"));
            mockFileSystem.AddFile(DirectoryWithPackages + mockFileSystem.Path.DirectorySeparatorChar + ProjectWithExe + ".csproj",
                                   new MockFileData($"<root xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">{GetTestProjectFileData(false, true)}</root>"));
            mockFileSystem.AddFile(DirectoryWithPackages + mockFileSystem.Path.DirectorySeparatorChar + EmptyProject + ".csproj",
                                   new MockFileData($"<root xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">{GetTestProjectFileData(false, false)}</root>"));
            mockFileSystem.AddFile(DirectoryWithPackages + mockFileSystem.Path.DirectorySeparatorChar + "packages.config",
                                   new MockFileData("<root><package id=\"" + PackageId + "\" version=\"" + PackageVersion + "\" /></root>"));

            mockFileSystem.AddFile(DirectoryWithoutPackages + mockFileSystem.Path.DirectorySeparatorChar + ProjectWithReferenceAndExe + ".csproj",
                                   new MockFileData($"<root xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">{GetTestProjectFileData(true, true)}</root>"));
            mockFileSystem.AddFile(DirectoryWithoutPackages + mockFileSystem.Path.DirectorySeparatorChar + ProjectWithReference + ".csproj",
                                   new MockFileData($"<root xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">{GetTestProjectFileData(true, false)}</root>"));
            mockFileSystem.AddFile(DirectoryWithoutPackages + mockFileSystem.Path.DirectorySeparatorChar + ProjectWithExe + ".csproj",
                                   new MockFileData($"<root xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">{GetTestProjectFileData(false, true)}</root>"));
            mockFileSystem.AddFile(DirectoryWithoutPackages + mockFileSystem.Path.DirectorySeparatorChar + EmptyProject + ".csproj",
                                   new MockFileData($"<root xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">{GetTestProjectFileData(false, false)}</root>"));

            mockFileSystem.AddFile(DirectoryWithPackages + mockFileSystem.Path.DirectorySeparatorChar + MultipleReferenceProject + ".csproj",
                                   new MockFileData($"<root xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">{GetTestProjectFileData(true, true)}{GetTestProjectFileData(true, false)}</root>"));

            return mockFileSystem;
        }

        private static string GetTestProjectFileData(bool hasReference, bool isExecutable)
        {
            var referenceTag = hasReference ? "<ProjectReference Include=\"" + ReferencedProjectPath + "\"></ProjectReference>" : string.Empty;
            var outputType = isExecutable ? "<OutputType>Exe</OutputType>" : string.Empty;
            return $"{referenceTag}{outputType}";
        }
    }
}
