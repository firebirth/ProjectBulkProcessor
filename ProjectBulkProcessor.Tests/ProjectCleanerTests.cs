using System.IO.Abstractions.TestingHelpers;
using ProjectBulkProcessor.Tests.Assertions;
using ProjectBulkProcessor.Upgrade.Processors;
using Xunit;

namespace ProjectBulkProcessor.Tests
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
    }
}
