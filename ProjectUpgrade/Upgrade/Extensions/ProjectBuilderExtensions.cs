using System.Collections.Generic;
using ProjectUpgrade.Models;
using ProjectUpgrade.Upgrade.Interfaces;

namespace ProjectUpgrade.Upgrade.Extensions
{
    public static class ProjectBuilderExtensions
    {
        public static IProjectBuilder AddPackageDependencies(this IProjectBuilder builder,
                                                             IEnumerable<PackageDependencyModel> packageDependencies)
        {
            var groupBuilder = builder.AddItemGroup();
            foreach (var dependency in packageDependencies)
            {
                groupBuilder.WithElement("PackageReference")
                            .WithAttribute("Include", dependency.PackageId)
                            .WithAttribute("Version", dependency.Version);
            }

            return builder;
        }

        public static IProjectBuilder AddProjectReferences(this IProjectBuilder builder,
                                                           IEnumerable<ProjectReferenceModel> projectReferences)
        {
            var groupBuilder = builder.AddItemGroup();
            foreach (var reference in projectReferences)
            {
                groupBuilder.WithElement("ProjectReference")
                            .WithAttribute("Include", reference.RelativePath);
            }

            return builder;
        }

        public static IProjectBuilder SetProjectType(this IProjectBuilder builder,
                                                     bool isExecutable)
        {
            if (isExecutable)
            {
                builder.AddPropertyGroup()
                       .WithElement("OutputType")
                       .WithNodeValue("Exe");
            }

            return builder;
        }
    }
}
