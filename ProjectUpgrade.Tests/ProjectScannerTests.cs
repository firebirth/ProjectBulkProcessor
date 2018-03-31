using System;
using System.Collections;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Moq;
using ProjectUpgrade.Interfaces;
using ProjectUpgrade.Processors;
using Xunit;

namespace ProjectUpgrade.Tests
{
    public class ProjectScannerTests
    {
        private const string ExistingDirectory = "existingDir";
        private const string ExistingEmptyDirectory = "existingEmptyDir";
        private const string NonExistingDirectory = "nonExistingDir";
        private const int FileCount = 10;
        private readonly IProjectScanner _sut;
        private readonly Mock<IProjectParser> _projectParserMock;

        public ProjectScannerTests()
        {
            _projectParserMock = new Mock<IProjectParser>();

            var fileSystemMock = new MockFileSystem();
            fileSystemMock.AddDirectory(ExistingEmptyDirectory);
            fileSystemMock.AddDirectory(ExistingDirectory);
            for (int i = 0; i < FileCount; i++)
            {
                var fileName = ExistingDirectory
                           + fileSystemMock.Path.DirectorySeparatorChar
                           + $"{i}.csproj";
                fileSystemMock.AddFile(fileName, new MockFileData($"<doc{i}></doc{i}>"));
            }

            _sut = new ProjectScanner(_projectParserMock.Object, fileSystemMock);
        }

        [Fact]
        public void ShouldThrowIfDirectoryWasNotFound()
        {
            Func<IEnumerable> sutAction = () => _sut.ScanForProjects(NonExistingDirectory);

            sutAction.Enumerating()
                     .Should().ThrowExactly<DirectoryNotFoundException>()
                     .WithMessage($"Directory {NonExistingDirectory} doesn't exist.");
        }

        [Fact]
        public void ShouldNotThrowIfProjectFilesWereNotFoundAndReturnEmptyEnumerable()
        {
            var result = _sut.ScanForProjects(ExistingEmptyDirectory);

            result.Should().BeEmpty();
        }

        [Fact]
        public void ShouldCallProjectParserForEachFoundFile()
        {
            var result = _sut.ScanForProjects(ExistingDirectory);

            result.Should().HaveCount(FileCount);

            _projectParserMock.Verify(parser => parser.ParseProject(It.IsAny<FileInfoBase>()), Times.Exactly(FileCount));
        }
    }
}
