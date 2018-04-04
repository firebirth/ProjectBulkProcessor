using System.IO;
using System.IO.Abstractions;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace ProjectUpgrade.Tests.Assertions
{
    public class DirectoryInfoBaseAssertions : ReferenceTypeAssertions<DirectoryInfoBase, DirectoryInfoBaseAssertions>
    {
        public DirectoryInfoBaseAssertions(DirectoryInfoBase directoryBase)
        {
            Subject = directoryBase;
        }

        protected override string Identifier { get; } = "directory";

        public AndConstraint<DirectoryInfoBaseAssertions> HaveFiles(string filePattern,
                                                                    SearchOption searchOption = SearchOption.AllDirectories,
                                                                    string because = "",
                                                                    params object[] becauseArgs)
        {
            Execute.Assertion
                   .BecauseOf(because, becauseArgs)
                   .Given(() => Subject.GetFiles(filePattern, searchOption))
                   .ForCondition(files => files.Any())
                   .FailWith(
                       $"Expected to have files with names like {filePattern} in {Subject.FullName}, but found none");

            return new AndConstraint<DirectoryInfoBaseAssertions>(this);
        }

        public AndConstraint<DirectoryInfoBaseAssertions> NotHaveFiles(string filePattern,
                                                                       SearchOption searchOption = SearchOption.AllDirectories,
                                                                       string because = "",
                                                                       params object[] becauseArgs)
        {
            Execute.Assertion
                   .BecauseOf(because, becauseArgs)
                   .Given(() => Subject.GetFiles(filePattern, searchOption))
                   .ForCondition(files => !files.Any())
                   .FailWith($"Expected to not have files with names like {filePattern} in {Subject.FullName}, but found {{0}}", files => files);

            return new AndConstraint<DirectoryInfoBaseAssertions>(this);
        }
    }



    public static class DirectoryInfoBaseAssertionsExtensions
    {
        public static DirectoryInfoBaseAssertions Should(this DirectoryInfoBase directoryInfo)
        {
            return new DirectoryInfoBaseAssertions(directoryInfo);
        }
    }
}
