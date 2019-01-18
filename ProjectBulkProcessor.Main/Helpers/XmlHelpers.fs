module XmlHelpers

open System.IO
open System.Xml
open System.Xml.Linq
open System.Xml.XPath

let private projectNamespaceManager = XmlNamespaceManager(NameTable())

do projectNamespaceManager.AddNamespace("project", "http://schemas.microsoft.com/developer/msbuild/2003")

let getProjectElementByName elementName xElement =
    Extensions.XPathSelectElement(xElement, "//project:" + elementName, projectNamespaceManager)
    |> NullHelper.handle

let getProjectElementsByName elementName xElement =
    Extensions.XPathSelectElements(xElement, "//project:" + elementName, projectNamespaceManager)

let getElementsByName elementName xElement =
    Extensions.XPathSelectElements(xElement, elementName)

let getAttribute attributeName (xElement : XElement) =
    XName.Get attributeName
    |> xElement.Attribute
    |> NullHelper.handle

let getAttributeValue attributeName xElement =
    getAttribute attributeName xElement
    |> Option.map (fun a -> a.Value |> NullHelper.handle)
    |> Option.flatten

let private mapElementsBase xdoc xPath elementSelector elementLookup =
    elementLookup xdoc xPath |> Seq.map elementSelector

let mapProjectElements elementSelector xdoc xPath =
    mapElementsBase xdoc xPath elementSelector getProjectElementsByName

let mapElements elementSelector xPath xdoc =
    mapElementsBase xdoc xPath elementSelector getElementsByName

let readXml filePath =
    using (File.OpenRead filePath) XDocument.Load
