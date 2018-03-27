using System.Linq;
using FluentAssertions;
using ProjectUpgrade.Processors;
using ProjectUpgrade.Tests.Assertions;
using Xunit;

namespace ProjectUpgrade.Tests
{
    public class ProjectBuilderTests
    {
        private readonly ProjectBuilder _sut;

        public ProjectBuilderTests()
        {
            _sut = new ProjectBuilder();
        }

        [Fact]
        public void ShouldHaveRootElement()
        {
            var result = _sut.Build();

            result.Should().HaveRoot("Project")
                  .Which.Should().HaveAttribute("Sdk", "Microsoft.NET.Sdk");
        }

        [Theory]
        [InlineData("TestGroup1")]
        [InlineData("TestGroup2")]
        public void AddGroupShouldAddGroupToRootElement(string groupName)
        {
            var result = _sut.AddGroup(groupName).Build();

            result.Should().HaveElement(groupName);
        }

        [Theory]
        [InlineData("TestGroup1", "TestElement1")]
        [InlineData("TestGroup2", "TestElement2")]
        public void AddingElementToGroupShouldAppendIt(string groupName, string elementName)
        {
            var result = _sut.AddGroup(groupName).WithElement(elementName).Build();

            result.Should().HaveElement(groupName)
                  .Which.Should().HaveElement(elementName);
        }

        [Theory]
        [InlineData("TestGroup1", "TestElement1", "NodeValue1")]
        [InlineData("TestGroup2", "TestElement2", "NodeValue2")]
        public void AddingNodeValueToElementToGroupShouldAppendIt(string groupName, string elementName, string nodeValue)
        {
            var result = _sut.AddGroup(groupName)
                             .WithElement(elementName)
                             .WithNodeValue(nodeValue)
                             .Build();

            result.Should().HaveElement(groupName)
                  .Which.Should().HaveElement(elementName)
                  .Which.Should().HaveValue(nodeValue);
        }

        [Theory]
        [InlineData("TestGroup1", "TestElement1", "AttributeName1", "AttributeValue1")]
        [InlineData("TestGroup2", "TestElement2", "AttributeName2", "AttributeValue2")]
        public void AddingAttributeToElementToGroupShouldAppendIt(string groupName, string elementName, string attributeName, string attributeValue)
        {
            var result = _sut.AddGroup(groupName)
                             .WithElement(elementName)
                             .WithAttribute(attributeName, attributeValue)
                             .Build();

            result.Should().HaveElement(groupName)
                  .Which.Should().HaveElement(elementName)
                  .Which.Should().HaveAttribute(attributeName, attributeValue);
        }

        [Theory]
        [InlineData("TestGroup1", "TestElement1", "AttributeName1", "AttributeValue1", "NodeValue1")]
        [InlineData("TestGroup2", "TestElement2", "AttributeName2", "AttributeValue2", "NodeValue2")]
        public void AddingAttributeAndNodeValueToElementToGroupShouldAppendIt(
            string groupName, string elementName, string attributeName, string attributeValue, string nodeValue)
        {
            var result = _sut.AddGroup(groupName)
                             .WithElement(elementName)
                             .WithAttribute(attributeName, attributeValue)
                             .WithNodeValue(nodeValue)
                             .Build();

            result.Should().HaveElement(groupName)
                  .Which.Should().HaveElement(elementName)
                  .Which.Should().HaveAttribute(attributeName, attributeValue)
                  .And.HaveValue(nodeValue);
        }

        [Theory]
        [InlineData("AttributeName1","AttributeName2","AttributeName3","AttributeName4","AttributeName5")]
        [InlineData("AttributeName1","AttributeName3","AttributeName4")]
        [InlineData("AttributeName1","AttributeName2")]
        public void AddingMultipleAttributeToElementToGroupShouldAppendAllOfThem(params string[] attributeNames)
        {
            var builder = _sut.AddGroup("TestGroup")
                              .WithElement("TestElement");
            foreach (var attributeName in attributeNames)
            {
                builder.WithAttribute(attributeName, "AttributeValue");
            }

            var result = builder.Build();

            result.Should().HaveElement("TestGroup")
                  .Which.Should().HaveElement("TestElement")
                  .Which.AttributesShould().HaveAllNames(attributeNames);
        }

        [Theory]
        [InlineData("NodeValue1","NodeValue2","NodeValue3","NodeValue4","NodeValue5")]
        [InlineData("NodeValue1","NodeValue3","NodeValue4")]
        [InlineData("NodeValue1","NodeValue2")]
        public void SettingMultipleNodeValuesToElementToGroupShouldOverwrite(params string[] nodeValues)
        {
            var builder = _sut.AddGroup("TestGroup")
                              .WithElement("TestElement");
            foreach (var attributeName in nodeValues)
            {
                builder.WithNodeValue(attributeName);
            }

            var result = builder.Build();

            result.Should().HaveElement("TestGroup")
                  .Which.Should().HaveElement("TestElement")
                  .Which.Should().HaveValue(nodeValues.Last());
        }
    }
}
