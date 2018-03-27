using System.Collections.Immutable;

namespace ProjectUpgrade.Models
{
    public class SolutionModel
    {
        public IImmutableList<ProjectModel> Projects { get; }

        public SolutionModel(IImmutableList<ProjectModel> projects)
        {
            Projects = projects;
        }
    }
}
