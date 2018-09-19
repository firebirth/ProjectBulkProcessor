using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using ProjectBulkProcessor.Shared.Models;
using ProjectBulkProcessor.Shared.Processors;
using ProjectBulkProcessor.Tests.Assertions;
using Xunit;

namespace ProjectBulkProcessor.Tests.Shared
{
    public class ProjectCleanerTests
    {
        private const string AssemblyInfoFileName = "AssemblyInfo.cs";
        private const string PackagesConfigFileName = "packages.config";
        private readonly MockFileSystem _fileSystem = new MockFileSystem();
        private readonly ProjectCleaner _sut;

        public ProjectCleanerTests()
        {
            _sut = new ProjectCleaner(_fileSystem);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test/innerDir")]
        [InlineData("test\\innerDir")]
        public void ShouldDeleteAssemlyInfo(string rootPath)
        {
            _fileSystem.AddDirectory(rootPath);
            _fileSystem.AddFile(rootPath + _fileSystem.Path.DirectorySeparatorChar + AssemblyInfoFileName,
                                MockFileData.NullObject);
            _fileSystem.AddFile(rootPath + _fileSystem.Path.DirectorySeparatorChar + "SomeFile",
                                MockFileData.NullObject);

            _sut.DeleteDeprecatedFiles(rootPath);

            _fileSystem.Should().HaveDirectory(rootPath)
                       .Which.Should().NotHaveFiles(AssemblyInfoFileName);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test/innerDir")]
        [InlineData("test\\innerDir")]
        public void ShouldDeleteFolderWithAssemlyInfoWhenItIsOnlyFile(string rootPath)
        {
            _fileSystem.AddDirectory(rootPath);
            _fileSystem.AddFile(rootPath + _fileSystem.Path.DirectorySeparatorChar + AssemblyInfoFileName,
                                MockFileData.NullObject);

            _sut.DeleteDeprecatedFiles(rootPath);

            _fileSystem.Should().NotHaveDirectory(rootPath);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test/innerDir")]
        [InlineData("test\\innerDir")]
        public void ShouldDeletePackagesConfig(string rootPath)
        {
            _fileSystem.AddDirectory(rootPath);
            _fileSystem.AddFile(rootPath + _fileSystem.Path.DirectorySeparatorChar + PackagesConfigFileName,
                                MockFileData.NullObject);

            _sut.DeleteDeprecatedFiles(rootPath);

            _fileSystem.Should().HaveDirectory(rootPath)
                       .Which.Should().NotHaveFiles(PackagesConfigFileName);
        }

        [Theory]
        [MemberData(nameof(TransitiveReferencesTestData))]
        public void ShouldRemoveTransitiveReferences(ImmutableList<ProjectModel> initial, ImmutableList<ProjectModel> expected, string reason)
        {
            var actual = _sut.CleanTransitiveReferences(initial);
            
            actual.Should().BeEquivalentTo(expected, o => o.Excluding(e => e.ProjectFile.Directory),reason);
        }

        public static IEnumerable<object[]> TransitiveReferencesTestData
        {
            get
            {
                yield return new object[] { ImmutableList<ProjectModel>.Empty, ImmutableList<ProjectModel>.Empty, "Empty should remain empty" };

                var singleProject = new ProjectModel(GetFileInfo("singleProject"), ImmutableList<ProjectReferenceModel>.Empty, ImmutableList<PackageDependencyModel>.Empty);
                yield return new object[] { new [] { singleProject }.ToImmutableList(), new [] { singleProject }.ToImmutableList(), "Single project should remain intact" };

                var mockFileSystemSimple = new MockFileSystem();
                var parentSimpleProjectActual = new ProjectModel(GetFileInfo("parentProject", mockFileSystemSimple), ImmutableList<ProjectReferenceModel>.Empty, new[] { new PackageDependencyModel("testPackage", "1")}.ToImmutableList());
                var childSimpleProjectActual = new ProjectModel(GetFileInfo("childProject", mockFileSystemSimple), new[] { new ProjectReferenceModel("../parentProject/parentProject.csproj") }.ToImmutableList(), new[] { new PackageDependencyModel("testPackage", "1") }.ToImmutableList());
                var parentSimpleProjectExpected = new ProjectModel(GetFileInfo("parentProject", mockFileSystemSimple), ImmutableList<ProjectReferenceModel>.Empty, new[] { new PackageDependencyModel("testPackage", "1") }.ToImmutableList());
                var childSimpleProjectExpected = new ProjectModel(GetFileInfo("childProject", mockFileSystemSimple), new[] { new ProjectReferenceModel("../parentProject/parentProject.csproj") }.ToImmutableList(), ImmutableList<PackageDependencyModel>.Empty);
                yield return new object[] { new[] { parentSimpleProjectActual, childSimpleProjectActual }.ToImmutableList(), new[] { parentSimpleProjectExpected, childSimpleProjectExpected }.ToImmutableList(), "Should remove single simple transitive dependency" };

                var mockFileSystemNested = new MockFileSystem();
                var parentNestedProjectActual = new ProjectModel(GetFileInfo("parentProject", mockFileSystemNested), ImmutableList<ProjectReferenceModel>.Empty, new[] { new PackageDependencyModel("testPackage", "1") }.ToImmutableList());
                var childNestedProject1Actual = new ProjectModel(GetFileInfo("childProject1", mockFileSystemNested), new[] { new ProjectReferenceModel("../parentProject/parentProject.csproj") }.ToImmutableList(), ImmutableList<PackageDependencyModel>.Empty);
                var childNestedProject2Actual = new ProjectModel(GetFileInfo("childProject2", mockFileSystemNested), new[] { new ProjectReferenceModel("../childProject1/childProject1.csproj") }.ToImmutableList(), new[] { new PackageDependencyModel("testPackage", "1") }.ToImmutableList());
                var parentNestedProjectExpected = new ProjectModel(GetFileInfo("parentProject", mockFileSystemNested), ImmutableList<ProjectReferenceModel>.Empty, new[] { new PackageDependencyModel("testPackage", "1") }.ToImmutableList());
                var childNestedProject1Expected = new ProjectModel(GetFileInfo("childProject1", mockFileSystemNested), new[] { new ProjectReferenceModel("../parentProject/parentProject.csproj") }.ToImmutableList(), ImmutableList<PackageDependencyModel>.Empty);
                var childNestedProject2Expected = new ProjectModel(GetFileInfo("childProject2", mockFileSystemNested), new[] { new ProjectReferenceModel("../childProject1/childProject1.csproj") }.ToImmutableList(), ImmutableList<PackageDependencyModel>.Empty);
                yield return new object[] { new[] { parentNestedProjectActual, childNestedProject1Actual, childNestedProject2Actual }.ToImmutableList(), new[] { parentNestedProjectExpected, childNestedProject1Expected, childNestedProject2Expected }.ToImmutableList(), "Should remove single nested transitive dependency" };

                var mockFileSystemMultiple = new MockFileSystem();
                var parentMultipleProjectActual = new ProjectModel(GetFileInfo("parentProject", mockFileSystemMultiple), ImmutableList<ProjectReferenceModel>.Empty, new[] { new PackageDependencyModel("testPackage", "1") }.ToImmutableList());
                var childMultipleProject1Actual = new ProjectModel(GetFileInfo("childProject1", mockFileSystemMultiple), new[] { new ProjectReferenceModel("../parentProject/parentProject.csproj") }.ToImmutableList(), new[] { new PackageDependencyModel("testPackage", "1") }.ToImmutableList());
                var childMultipleProject2Actual = new ProjectModel(GetFileInfo("childProject2", mockFileSystemMultiple), new[] { new ProjectReferenceModel("../parentProject/parentProject.csproj") }.ToImmutableList(), new[] { new PackageDependencyModel("testPackage", "1") }.ToImmutableList());
                var parentMultipleProjectExpected = new ProjectModel(GetFileInfo("parentProject", mockFileSystemMultiple), ImmutableList<ProjectReferenceModel>.Empty, new[] { new PackageDependencyModel("testPackage", "1") }.ToImmutableList());
                var childMultipleProject1Expected = new ProjectModel(GetFileInfo("childProject1", mockFileSystemMultiple), new[] { new ProjectReferenceModel("../parentProject/parentProject.csproj") }.ToImmutableList(), ImmutableList<PackageDependencyModel>.Empty);
                var childMultipleProject2Expected = new ProjectModel(GetFileInfo("childProject2", mockFileSystemMultiple), new[] { new ProjectReferenceModel("../parentProject/parentProject.csproj") }.ToImmutableList(), ImmutableList<PackageDependencyModel>.Empty);
                yield return new object[] { new[] { parentMultipleProjectActual, childMultipleProject1Actual, childMultipleProject2Actual }.ToImmutableList(), new[] { parentMultipleProjectExpected, childMultipleProject1Expected, childMultipleProject2Expected }.ToImmutableList(), "Should remove multiple transitive dependency" };
            }
        }

        private static FileInfoBase GetFileInfo(string projectName, IMockFileDataAccessor mockFileSystem = null)
        {
            mockFileSystem = mockFileSystem ?? new MockFileSystem();
            var projectFilePath = $"{projectName}/{projectName}.csproj";
            mockFileSystem.AddFile(projectFilePath, MockFileData.NullObject);
            return mockFileSystem.FileInfo.FromFileName(projectFilePath);
        }
    }
}
