using System.Linq;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using ProjectBulkProcessor.Upgrade.Models;

namespace ProjectBulkProcessor.Tests.Assertions
{
    public class OptionsModelAssertions : ReferenceTypeAssertions<OptionsModel, OptionsModelAssertions>
    {
        private static readonly PropertyInfo[] Properties = typeof(OptionsModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        public OptionsModelAssertions(OptionsModel model)
        {
            Subject = model;
        }
        
        protected override string Identifier => "optionsModel";

        public AndWhichConstraint<OptionsModelAssertions, string> HaveProperty(string path,
                                                                               string because = "",
                                                                               params object[] becauseArgs)
        {
            var propertyInfo = Properties.Single(p => string.Equals(p.Name, path));
            Execute.Assertion
                   .BecauseOf(because, becauseArgs)
                   .Given(() => propertyInfo)
                   .ForCondition(p => p.GetValue(Subject) != null)
                   .FailWith($"Value is not set for {path}");

            return new AndWhichConstraint<OptionsModelAssertions, string>(this, propertyInfo.GetValue(Subject).ToString());
        }
    }

    public static class OptionsModelAssertionsExtensions
    {
        public static OptionsModelAssertions Should(this OptionsModel fileSystem)
        {
            return new OptionsModelAssertions(fileSystem);
        }
    }
}
