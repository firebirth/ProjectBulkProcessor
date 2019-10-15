module XmlHelpers

open System.IO
open System.Xml
open System.Xml.Linq
open System.Xml.XPath

let private projectNamespaceManager = XmlNamespaceManager(NameTable())

do projectNamespaceManager.AddNamespace("project", "http://schemas.microsoft.com/developer/msbuild/2003")

let getProjectElementByName elementName xElement =
    Extensions.XPathSelectElement(xElement, "//project:" + elementName, projectNamespaceManager)
    |> Option.ofObj

let getProjectElementsByName elementName xElement =
    Extensions.XPathSelectElements(xElement, "//project:" + elementName, projectNamespaceManager)
    |> List.ofSeq

let getElementsByName elementName xElement =
    Extensions.XPathSelectElements(xElement, elementName)
    |> List.ofSeq

let getAttribute attributeName (xElement : XElement) =
    XName.Get attributeName
    |> xElement.Attribute
    |> Option.ofObj

let getAttributeValue attributeName xElement =
    let mapAttributeValue (attr: XAttribute) = attr.Value |> Option.ofObj
    getAttribute attributeName xElement
    |> Option.map mapAttributeValue
    |> Option.flatten

let private mapElementsBase elementLookup elementSelector xdoc xPath =
    elementLookup xdoc xPath
    |> List.map elementSelector

let mapProjectElements elementSelector xdoc xPath = mapElementsBase getProjectElementsByName elementSelector xdoc xPath 

let mapElements elementSelector xdoc xPath = mapElementsBase getElementsByName elementSelector xdoc xPath 

let readXml filePath =
    use fileStream = File.OpenRead filePath
    XDocument.Load fileStream
