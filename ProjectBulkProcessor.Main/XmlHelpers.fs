module XmlHelpers

open System.Xml
open System.Xml.Linq
open System.Xml.XPath

let private projectNamespaceManager = new XmlNamespaceManager(new NameTable())
do
    projectNamespaceManager.AddNamespace("project", "http://schemas.microsoft.com/developer/msbuild/2003")

let getProjectElementByName (xElement: XNode) elementName =
    match xElement.XPathSelectElement("//project:" + elementName, projectNamespaceManager) with
    | null -> None
    | xe -> Some (xe)

let getProjectElementsByName (xElement: XNode) elementName =
    match xElement.XPathSelectElements("//project:" + elementName, projectNamespaceManager) with
    | null -> None
    | xe -> Some (Array.ofSeq xe)

let getAttribute (xElement: XElement) attributeName =
    match xElement.Attribute <| XName.Get attributeName with
    | null -> None
    | attribute -> Some (attribute)

let getAttributeValue xElement attributeName =
    match getAttribute xElement attributeName with
    | None -> None
    | Some attribute -> match attribute.Value with
                        | null -> None
                        | value -> Some (value)

let mapElements xdoc xPath elementSelector =
    match getProjectElementsByName xdoc xPath with
    | Some elements -> Some (Array.map elementSelector elements)
    | None -> None