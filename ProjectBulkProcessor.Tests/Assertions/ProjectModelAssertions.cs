using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using ProjectBulkProcessor.Shared.Models;

namespace ProjectBulkProcessor.Tests.Assertions
{
    public class ProjectModelAssertions : ReferenceTypeAssertions<ProjectModel, ProjectModelAssertions>
    {
        public ProjectModelAssertions(ProjectModel subject)
        {
            Subject = subject;
        }

        protected override string Identifier { get; } = "projectModel";

        public bool HasProjectReference { get; private set; }
        public bool HasPackageDependency { get; private set; }

        public AndThenConstraint<ProjectModelAssertions> WhenShouldHaveProjectReference(bool hasProjectReference = true)
        {
            HasProjectReference = hasProjectReference;

            return new AndThenConstraint<ProjectModelAssertions>(this);
        }

        public AndThenConstraint<ProjectModelAssertions> WhenShouldHavePackageDependency(bool hasPackageDependency = true)
        {
            HasPackageDependency = hasPackageDependency;

            return new AndThenConstraint<ProjectModelAssertions>(this);
        }

        public AndConstraint<ProjectModelAssertions> HaveLocation(string expectedPath, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                   .BecauseOf(because, becauseArgs)
                   .Given(() => Subject.ProjectFile.FullName)
                   .ForCondition(actualPath => string.Equals(actualPath, expectedPath, StringComparison.OrdinalIgnoreCase))
                   .FailWith($"Expected project to be located in {expectedPath}, but it was {{0}}", actualPath => actualPath);

            return new AndConstraint<ProjectModelAssertions>(this);
        }

        public AndConstraint<ProjectModelAssertions> HaveProjectReference(string expectedProjectReferencePath, string because = "", params object[] becauseArgs)
        {
            var givenSelector = Execute.Assertion
                                       .BecauseOf(because, becauseArgs)
                                       .Given(() => Subject.ProjectReferences);

            if (HasProjectReference)
            {
                var expectedProjectReference = new ProjectReferenceModel(expectedProjectReferencePath);
                givenSelector
                    .ForCondition(actualReferences => actualReferences.Any(r => r.Equals(expectedProjectReference)))
                    .FailWith(
                        $"Expected project to reference {expectedProjectReferencePath} project, but it was referencing {{0}}",
                        actualReferences => actualReferences);
            }
            else
            {
                givenSelector
                    .ForCondition(actualReferences => !actualReferences.Any())
                    .FailWith("Expected project to have no references, but it was referencing {0}",
                              actualReferences => actualReferences);
            }

            return new AndConstraint<ProjectModelAssertions>(this);
        }

        public AndConstraint<ProjectModelAssertions> HavePackageDependency(string expectedPackageId,
                                                                           string expectedPackageVersion,
                                                                           string because = "",
                                                                           params object[] becauseArgs)
        {
            var givenSelector = Execute.Assertion
                                       .BecauseOf(because, becauseArgs)
                                       .Given(() => Subject.PackageDependencies);
            if (HasPackageDependency)
            {
                var expectedDependency = new PackageDependencyModel(expectedPackageId, expectedPackageVersion);
                givenSelector
                    .ForCondition(actualDependencies => actualDependencies.Any(r => r.Equals(expectedDependency)))
                    .FailWith(
                        $"Expected project to be have dependency on package {expectedPackageId}, version {expectedPackageVersion}, " +
                        "but it was depending on {0}", actualDependencies => actualDependencies);
            }
            else
            {
                givenSelector
                    .ForCondition(actualDependencies => !actualDependencies.Any())
                    .FailWith("Expected project to be have no dependencies, but it was depending on {0}", actualDependencies => actualDependencies);
            }

            return new AndConstraint<ProjectModelAssertions>(this);
        }
    }

    public static class ProjectModelAssertionsExtensions
    {
        public static ProjectModelAssertions Should(this ProjectModel projectModel)
        {
            return new ProjectModelAssertions(projectModel);
        }
    }
}
