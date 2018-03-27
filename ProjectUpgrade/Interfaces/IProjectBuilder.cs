using System.Xml.Linq;

namespace ProjectUpgrade.Interfaces
{
    public interface IProjectBuilder
    {
        IProjectGroupBuilder AddGroup(string groupName);

        XDocument Build();
    }

    public interface IProjectGroupBuilder : IProjectBuilder
    {
        IProjectElementBuilder WithElement(string elementName);
    }

    public interface IProjectElementBuilder : IProjectBuilder
    {
        IProjectElementBuilder WithNodeValue(string nodeValue);

        IProjectElementBuilder WithAttribute(string attributeName, string attributeValue);
    }
}
