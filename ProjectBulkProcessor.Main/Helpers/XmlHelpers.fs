module XmlHelpers

open System.Xml
open System.Xml.Linq
open System.Xml.XPath

let private projectNamespaceManager = new XmlNamespaceManager(new NameTable())
do
    projectNamespaceManager.AddNamespace("project", "http://schemas.microsoft.com/developer/msbuild/2003")

let private nullHandler value =
    match value with
    | null -> None
    | value -> Some value

let getProjectElementByName (xElement: XNode) elementName =
    xElement.XPathSelectElement("//project:" + elementName, projectNamespaceManager) |> nullHandler

let getProjectElementsByName (xElement: XNode) elementName =
    xElement.XPathSelectElements("//project:" + elementName, projectNamespaceManager)
    |> nullHandler
    |> Option.map Array.ofSeq

let getElementByName (xElement: XNode) elementName =
    xElement.XPathSelectElement elementName
    |> nullHandler

let getElementsByName (xElement: XNode) elementName =
    xElement.XPathSelectElements elementName
    |> nullHandler
    |> Option.map Array.ofSeq

let getAttribute (xElement: XElement) attributeName =
    XName.Get attributeName
    |> xElement.Attribute
    |> nullHandler

let getAttributeValue xElement attributeName =
    getAttribute xElement attributeName
    |> Option.map (fun a -> a.Value |> nullHandler)
    |> Option.flatten

let private mapElementsBase xdoc xPath elementSelector elementLookup =
    elementLookup xdoc xPath
    |> Option.map (Array.map elementSelector)

let mapProjectElements elementSelector xdoc xPath =
    mapElementsBase xdoc xPath elementSelector getProjectElementsByName
    |> Option.map OptionHelper.filterNones

let mapElements elementSelector xPath xdoc =
    mapElementsBase xdoc xPath elementSelector getElementsByName
    |> Option.map OptionHelper.filterNones
