using System.Xml.Linq;

namespace ProjectBulkProcessor.Upgrade.Interfaces
{
    public interface IProjectBuilder
    {
        IProjectGroupBuilder AddItemGroup();
        IProjectGroupBuilder AddPropertyGroup();
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
