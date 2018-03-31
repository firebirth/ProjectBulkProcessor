using System.IO;
using System.IO.Abstractions;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace ProjectUpgrade.Tests.Assertions
{
    public class FileSystemAssertions : ReferenceTypeAssertions<IFileSystem, FileSystemAssertions>
    {
        public FileSystemAssertions(IFileSystem fileSystem)
        {
            Subject = fileSystem;
        }

        protected override string Identifier { get; } = "fileSystem";

        public AndWhichConstraint<FileSystemAssertions, DirectoryInfoBase> NotHaveDirectory(string path,
                                                                                            string because = "",
                                                                                            params object[] becauseArgs)
        {
            Execute.Assertion
                   .BecauseOf(because, becauseArgs)
                   .Given(() => Subject.DirectoryInfo.FromDirectoryName(path))
                   .ForCondition(di => !di.Exists)
                   .FailWith($"Expected directory not to exist in {path}.");

            return new AndWhichConstraint<FileSystemAssertions, DirectoryInfoBase>(this, Subject.DirectoryInfo.FromDirectoryName(path));
        }

        public AndWhichConstraint<FileSystemAssertions, DirectoryInfoBase> HaveDirectory(string path,
                                                                                         string because = "",
                                                                                         params object[] becauseArgs)
        {
            Execute.Assertion
                   .BecauseOf(because, becauseArgs)
                   .Given(() => Subject.DirectoryInfo.FromDirectoryName(path))
                   .ForCondition(di => di.Exists)
                   .FailWith($"Expected directory to exist in {path}.");

            return new AndWhichConstraint<FileSystemAssertions, DirectoryInfoBase>(this, Subject.DirectoryInfo.FromDirectoryName(path));
        }
    }

    public static class FileSystemAssertionsExtensions
    {
        public static FileSystemAssertions Should(this IFileSystem fileSystem)
        {
            return new FileSystemAssertions(fileSystem);
        }
    }
}
