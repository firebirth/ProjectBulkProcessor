using System.IO.Abstractions.TestingHelpers;
using Moq;
using ProjectUpgrade.Interfaces;
using ProjectUpgrade.Processors;

namespace ProjectUpgrade.Tests
{
    public class UpgradeProcessorTests
    {
        private UpgradeProcessor _sut;

        public UpgradeProcessorTests()
        {
            var fileSystemMock = new MockFileSystem();
            var scannerMock = new Mock<IProjectScanner>();

            _sut = new UpgradeProcessor(fileSystemMock, scannerMock.Object);
        }
    }
}
