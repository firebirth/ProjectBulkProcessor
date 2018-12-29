module XmlHelpers

open System.Xml
open System.Xml.Linq
open System.Xml.XPath
open System.IO

let private projectNamespaceManager = new XmlNamespaceManager(new NameTable())
do
    projectNamespaceManager.AddNamespace("project", "http://schemas.microsoft.com/developer/msbuild/2003")

let getProjectElementByName (xElement: XNode) elementName =
    xElement.XPathSelectElement("//project:" + elementName, projectNamespaceManager)
    |> NullHelper.handle

let getProjectElementsByName (xElement: XNode) elementName =
    xElement.XPathSelectElements("//project:" + elementName, projectNamespaceManager)

let getElementByName (xElement: XNode) elementName =
    xElement.XPathSelectElement elementName
    |> NullHelper.handle

let getElementsByName (xElement: XNode) elementName =
    xElement.XPathSelectElements elementName

let getAttribute (xElement: XElement) attributeName =
    XName.Get attributeName
    |> xElement.Attribute
    |> NullHelper.handle

let getAttributeValue xElement attributeName =
    getAttribute xElement attributeName
    |> Option.map (fun a -> a.Value |> NullHelper.handle)
    |> Option.flatten

let private mapElementsBase xdoc xPath elementSelector elementLookup =
    elementLookup xdoc xPath
    |> Seq.map elementSelector

let mapProjectElements elementSelector xdoc xPath =
    mapElementsBase xdoc xPath elementSelector getProjectElementsByName

let mapElements elementSelector xPath xdoc =
    mapElementsBase xdoc xPath elementSelector getElementsByName

let readXml filePath =
    using (File.OpenRead filePath) XDocument.Load
