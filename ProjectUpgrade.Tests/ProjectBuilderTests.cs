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

        [Fact]
        public void AddItemGroupShouldAddGroupToRootElement()
        {
            var result = _sut.AddItemGroup().Build();

            result.Should().HaveElement("ItemGroup");
        }

        [Fact]
        public void AddPropertyGroupShouldAddGroupToRootElement()
        {
            var result = _sut.AddPropertyGroup().Build();

            result.Should().HaveElement("PropertyGroup");
        }

        [Theory]
        [InlineData("TestElement1")]
        [InlineData("TestElement2")]
        public void AddingElementToItemGroupShouldAppendIt(string elementName)
        {
            var result = _sut.AddItemGroup().WithElement(elementName).Build();

            result.Should().HaveElement("ItemGroup")
                  .Which.Should().HaveElement(elementName);
        }

        [Theory]
        [InlineData("TestElement1")]
        [InlineData("TestElement2")]
        public void AddingElementToPropertyGroupShouldAppendIt(string elementName)
        {
            var result = _sut.AddPropertyGroup().WithElement(elementName).Build();

            result.Should().HaveElement("PropertyGroup")
                  .Which.Should().HaveElement(elementName);
        }

        [Theory]
        [InlineData("TestElement2", "NodeValue2")]
        [InlineData("TestElement1", "NodeValue1")]
        public void AddingNodeValueToElementToItemGroupShouldAppendIt(string elementName, string nodeValue)
        {
            var result = _sut.AddItemGroup()
                             .WithElement(elementName)
                             .WithNodeValue(nodeValue)
                             .Build();

            result.Should().HaveElement("ItemGroup")
                  .Which.Should().HaveElement(elementName)
                  .Which.Should().HaveValue(nodeValue);
        }
         [Theory]
        [InlineData("TestElement2", "NodeValue2")]
        [InlineData("TestElement1", "NodeValue1")]
        public void AddingNodeValueToElementToPropertyGroupShouldAppendIt(string elementName, string nodeValue)
        {
            var result = _sut.AddPropertyGroup()
                             .WithElement(elementName)
                             .WithNodeValue(nodeValue)
                             .Build();

            result.Should().HaveElement("PropertyGroup")
                  .Which.Should().HaveElement(elementName)
                  .Which.Should().HaveValue(nodeValue);
        }


        [Theory]
        [InlineData("TestElement1", "AttributeName1", "AttributeValue1")]
        [InlineData("TestElement2", "AttributeName2", "AttributeValue2")]
        public void AddingAttributeToElementToItemGroupShouldAppendIt(string elementName, string attributeName, string attributeValue)
        {
            var result = _sut.AddItemGroup()
                             .WithElement(elementName)
                             .WithAttribute(attributeName, attributeValue)
                             .Build();

            result.Should().HaveElement("ItemGroup")
                  .Which.Should().HaveElement(elementName)
                  .Which.Should().HaveAttribute(attributeName, attributeValue);
        }

        [Theory]
        [InlineData("TestElement1", "AttributeName1", "AttributeValue1")]
        [InlineData("TestElement2", "AttributeName2", "AttributeValue2")]
        public void AddingAttributeToElementToPropertyGroupShouldAppendIt(string elementName, string attributeName, string attributeValue)
        {
            var result = _sut.AddPropertyGroup()
                             .WithElement(elementName)
                             .WithAttribute(attributeName, attributeValue)
                             .Build();

            result.Should().HaveElement("PropertyGroup")
                  .Which.Should().HaveElement(elementName)
                  .Which.Should().HaveAttribute(attributeName, attributeValue);
        }

        [Theory]
        [InlineData("TestElement1", "AttributeName1", "AttributeValue1", "NodeValue1")]
        [InlineData("TestElement2", "AttributeName2", "AttributeValue2", "NodeValue2")]
        public void AddingAttributeAndNodeValueToElementToItemGroupShouldAppendIt(
            string elementName, string attributeName, string attributeValue, string nodeValue)
        {
            var result = _sut.AddItemGroup()
                             .WithElement(elementName)
                             .WithAttribute(attributeName, attributeValue)
                             .WithNodeValue(nodeValue)
                             .Build();

            result.Should().HaveElement("ItemGroup")
                  .Which.Should().HaveElement(elementName)
                  .Which.Should().HaveAttribute(attributeName, attributeValue)
                  .And.HaveValue(nodeValue);
        }

        [Theory]
        [InlineData("TestElement1", "AttributeName1", "AttributeValue1", "NodeValue1")]
        [InlineData("TestElement2", "AttributeName2", "AttributeValue2", "NodeValue2")]
        public void AddingAttributeAndNodeValueToElementToPropertyGroupShouldAppendIt(
            string elementName, string attributeName, string attributeValue, string nodeValue)
        {
            var result = _sut.AddPropertyGroup()
                             .WithElement(elementName)
                             .WithAttribute(attributeName, attributeValue)
                             .WithNodeValue(nodeValue)
                             .Build();

            result.Should().HaveElement("PropertyGroup")
                  .Which.Should().HaveElement(elementName)
                  .Which.Should().HaveAttribute(attributeName, attributeValue)
                  .And.HaveValue(nodeValue);
        }

        [Theory]
        [InlineData("AttributeName1","AttributeName2","AttributeName3","AttributeName4","AttributeName5")]
        [InlineData("AttributeName1","AttributeName3","AttributeName4")]
        [InlineData("AttributeName1","AttributeName2")]
        public void AddingMultipleAttributeToElementToItemGroupShouldAppendAllOfThem(params string[] attributeNames)
        {
            var builder = _sut.AddItemGroup()
                              .WithElement("TestElement");
            foreach (var attributeName in attributeNames)
            {
                builder.WithAttribute(attributeName, "AttributeValue");
            }

            var result = builder.Build();

            result.Should().HaveElement("ItemGroup")
                  .Which.Should().HaveElement("TestElement")
                  .Which.AttributesShould().HaveAllNames(attributeNames);
        }

        [Theory]
        [InlineData("AttributeName1","AttributeName2","AttributeName3","AttributeName4","AttributeName5")]
        [InlineData("AttributeName1","AttributeName3","AttributeName4")]
        [InlineData("AttributeName1","AttributeName2")]
        public void AddingMultipleAttributeToElementToPropertyGroupShouldAppendAllOfThem(params string[] attributeNames)
        {
            var builder = _sut.AddPropertyGroup()
                              .WithElement("TestElement");
            foreach (var attributeName in attributeNames)
            {
                builder.WithAttribute(attributeName, "AttributeValue");
            }

            var result = builder.Build();

            result.Should().HaveElement("PropertyGroup")
                  .Which.Should().HaveElement("TestElement")
                  .Which.AttributesShould().HaveAllNames(attributeNames);
        }

        [Theory]
        [InlineData("NodeValue1","NodeValue2","NodeValue3","NodeValue4","NodeValue5")]
        [InlineData("NodeValue1","NodeValue3","NodeValue4")]
        [InlineData("NodeValue1","NodeValue2")]
        public void SettingMultipleNodeValuesToElementToItemGroupShouldOverwrite(params string[] nodeValues)
        {
            var builder = _sut.AddItemGroup()
                              .WithElement("TestElement");
            foreach (var attributeName in nodeValues)
            {
                builder.WithNodeValue(attributeName);
            }

            var result = builder.Build();

            result.Should().HaveElement("ItemGroup")
                  .Which.Should().HaveElement("TestElement")
                  .Which.Should().HaveValue(nodeValues.Last());
        }

        [Theory]
        [InlineData("NodeValue1","NodeValue2","NodeValue3","NodeValue4","NodeValue5")]
        [InlineData("NodeValue1","NodeValue3","NodeValue4")]
        [InlineData("NodeValue1","NodeValue2")]
        public void SettingMultipleNodeValuesToElementToPropertyGroupShouldOverwrite(params string[] nodeValues)
        {
            var builder = _sut.AddPropertyGroup()
                              .WithElement("TestElement");
            foreach (var attributeName in nodeValues)
            {
                builder.WithNodeValue(attributeName);
            }

            var result = builder.Build();

            result.Should().HaveElement("PropertyGroup")
                  .Which.Should().HaveElement("TestElement")
                  .Which.Should().HaveValue(nodeValues.Last());
        }
    }
}
