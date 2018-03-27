using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using ProjectUpgrade.Processors;
using ProjectUpgrade.Tests.Assertions;
using Xunit;

namespace ProjectUpgrade.Tests
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

        public ProjectParserTests()
        {
            _sut = new ProjectParser();
            _fileSystem = SetFileSystem();
        }

        [Theory]
        [InlineData(DirectoryWithPackages, ProjectWithReferenceAndExe, true, true, true)]
        [InlineData(DirectoryWithPackages, ProjectWithReference, true, true, false)]
        [InlineData(DirectoryWithPackages, ProjectWithExe, false, true, true)]
        [InlineData(DirectoryWithPackages, EmptyProject, false, true, false)]
        [InlineData(DirectoryWithoutPackages, ProjectWithReferenceAndExe, true, false, true)]
        [InlineData(DirectoryWithoutPackages, ProjectWithReference, true, false, false)]
        [InlineData(DirectoryWithoutPackages, ProjectWithExe, false, false, true)]
        [InlineData(DirectoryWithoutPackages, EmptyProject, false, false, false)]
        public void ShouldParseProjectWithPackageDependencyAndProjectReferenceAndOfExecutableType(string directoryName,
                                                                                        string projectName,
                                                                                        bool hasProjectReference,
                                                                                        bool hasPackageDependency,
                                                                                        bool expectedIsExecutable)
        {
            var fileName = _fileSystem.AllFiles.Single(f => f.EndsWith(".csproj") && f.Contains(projectName) && f.Contains(directoryName));
            var fileInfo = _fileSystem.FileInfo.FromFileName(fileName);

            var result = _sut.ParseProject(fileInfo);

            result.Should()
                  .WhenShouldHavePackageDependency(hasPackageDependency)
                  .And.WhenShouldHaveProjectReference(hasProjectReference)
                  .Then.BeExecutable(expectedIsExecutable)
                  .And.HaveLocation(fileName)
                  .And.HaveProjectReference(ReferencedProjectPath)
                  .And.HavePackageDependency(PackageId, PackageVersion);
        }

        [Fact]
        public void ShouldParseMultipleReferences()
        {
            var fileName = _fileSystem.AllFiles.Single(f => f.EndsWith(".csproj") && f.Contains(MultipleReferenceProject) && f.Contains(DirectoryWithPackages));
            var fileInfo = _fileSystem.FileInfo.FromFileName(fileName);

            var result = _sut.ParseProject(fileInfo);

            result.ProjectReferences.Should().HaveCount(2);
        }

        private static MockFileSystem SetFileSystem()
        {
            var mockFileSystem = new MockFileSystem();

            mockFileSystem.AddDirectory(DirectoryWithPackages);
            mockFileSystem.AddDirectory(DirectoryWithoutPackages);

            mockFileSystem.AddFile(DirectoryWithPackages + mockFileSystem.Path.DirectorySeparatorChar + ProjectWithReferenceAndExe + ".csproj",
                                   new MockFileData($"<root>{GetTestProjectFileData(true, true)}</root>"));
            mockFileSystem.AddFile(DirectoryWithPackages + mockFileSystem.Path.DirectorySeparatorChar + ProjectWithReference + ".csproj",
                                   new MockFileData($"<root>{GetTestProjectFileData(true, false)}</root>"));
            mockFileSystem.AddFile(DirectoryWithPackages + mockFileSystem.Path.DirectorySeparatorChar + ProjectWithExe + ".csproj",
                                   new MockFileData($"<root>{GetTestProjectFileData(false, true)}</root>"));
            mockFileSystem.AddFile(DirectoryWithPackages + mockFileSystem.Path.DirectorySeparatorChar + EmptyProject + ".csproj",
                                   new MockFileData($"<root>{GetTestProjectFileData(false, false)}</root>"));
            mockFileSystem.AddFile(DirectoryWithPackages + mockFileSystem.Path.DirectorySeparatorChar + "packages.config",
                                   new MockFileData("<root><package id=\"" + PackageId + "\" version=\"" + PackageVersion + "\" /></root>"));

            mockFileSystem.AddFile(DirectoryWithoutPackages + mockFileSystem.Path.DirectorySeparatorChar + ProjectWithReferenceAndExe + ".csproj",
                                   new MockFileData($"<root>{GetTestProjectFileData(true, true)}</root>"));
            mockFileSystem.AddFile(DirectoryWithoutPackages + mockFileSystem.Path.DirectorySeparatorChar + ProjectWithReference + ".csproj",
                                   new MockFileData($"<root>{GetTestProjectFileData(true, false)}</root>"));
            mockFileSystem.AddFile(DirectoryWithoutPackages + mockFileSystem.Path.DirectorySeparatorChar + ProjectWithExe + ".csproj",
                                   new MockFileData($"<root>{GetTestProjectFileData(false, true)}</root>"));
            mockFileSystem.AddFile(DirectoryWithoutPackages + mockFileSystem.Path.DirectorySeparatorChar + EmptyProject + ".csproj",
                                   new MockFileData($"<root>{GetTestProjectFileData(false, false)}</root>"));

            mockFileSystem.AddFile(DirectoryWithPackages + mockFileSystem.Path.DirectorySeparatorChar + MultipleReferenceProject + ".csproj",
                                   new MockFileData($"<root>{GetTestProjectFileData(true, true)}{GetTestProjectFileData(true, false)}</root>"));

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
