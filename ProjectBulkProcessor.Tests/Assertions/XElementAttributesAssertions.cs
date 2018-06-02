using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Xml;

namespace ProjectBulkProcessor.Tests.Assertions
{
    public class XElementAttributesAssertions : XElementAssertions
    {
        public XElementAttributesAssertions(XElement xElement) : base(xElement)
        {
        }

        public AndConstraint<XElementAttributesAssertions> HaveAllNames(string[] expectedAttributeNames,  string because = "", params object[] becauseArgs)
        {
            Execute.Assertion.BecauseOf(because, becauseArgs)
                   .Given(() => Subject.Attributes().Select(a => a.Name.LocalName))
                   .ForCondition(actualAttributeNames => actualAttributeNames.Intersect(expectedAttributeNames).Any())
                   .FailWith($"Expected element to contain all of attribute names in {string.Join(";", expectedAttributeNames)}, but found {{0}}", attributes => attributes);

            return new AndConstraint<XElementAttributesAssertions>(this);
        }
    }

    public static class XElementAssertionsExtensions
    {
        public static XElementAttributesAssertions AttributesShould(this XElement element)
        {
            return new XElementAttributesAssertions(element);
        }
    }
}
