using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Moq;
using ProjectUpgrade.Configration;
using ProjectUpgrade.Interfaces;
using ProjectUpgrade.Models;
using ProjectUpgrade.Processors;
using Xunit;

namespace ProjectUpgrade.Tests
{
    public class UpgradeProcessorTests
    {
        private readonly UpgradeProcessor _sut;
        private readonly Mock<IProjectScanner> _scannerMock;
        private readonly Mock<IProjectCleaner> _cleanerMock;

        public UpgradeProcessorTests()
        {
            _scannerMock = new Mock<IProjectScanner>();
            _cleanerMock = new Mock<IProjectCleaner>();

            _scannerMock.Setup(s => s.ScanForProjects(It.IsAny<string>())).Returns(Enumerable.Empty<ProjectModel>());

            _sut = new UpgradeProcessor(_scannerMock.Object, _cleanerMock.Object);
        }

        [Theory]
        [InlineData("randomString")]
        [InlineData("dev/abc/xyz")]
        public void ShouldCallScanForProjectsAndNotDeleteDeprecatedFilesWhenNoProjectsAreFound(string rootPath)
        {
            var upgradeParameters = new UpgradeParameters(rootPath);

            _sut.ProcessProjects(upgradeParameters);

            _scannerMock.Verify(s => s.ScanForProjects(rootPath), Times.Once);
            _cleanerMock.Verify(s => s.DeleteDeprecatedFiles(rootPath), Times.Never);
        }

        [Fact]
        public void ShouldCallScanForProjectsAndDeleteDeprecatedFilesAndWriteUpdatedFilesWhenProjectsAreFound()
        {
            var rootPath = "Path/With/Projects";
            var projectFileMock = new Mock<FileInfoBase>();
            projectFileMock.Setup(p => p.OpenWrite()).Returns(Stream.Null);
            var projectModel = new ProjectModel(projectFileMock.Object, 
                                                Enumerable.Empty<ProjectReferenceModel>().ToImmutableList(),
                                                Enumerable.Empty<PackageDependencyModel>().ToImmutableList(), 
                                                false);
            _scannerMock.Setup(s => s.ScanForProjects(rootPath)).Returns(new[] {projectModel});

            var upgradeParameters = new UpgradeParameters(rootPath);

            _sut.ProcessProjects(upgradeParameters);

            _scannerMock.Verify(s => s.ScanForProjects(rootPath), Times.Once);
            _cleanerMock.Verify(c => c.DeleteDeprecatedFiles(rootPath), Times.Once);
            projectFileMock.Verify(f => f.OpenWrite(), Times.Once);
        }
    }
}
