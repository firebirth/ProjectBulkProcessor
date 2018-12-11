module XmlHelpers

open System.Xml
open System.Xml.Linq
open System.Xml.XPath

let private projectNamespaceManager = new XmlNamespaceManager(new NameTable())
do
    projectNamespaceManager.AddNamespace("project", "http://schemas.microsoft.com/developer/msbuild/2003")

let getProjectElementByName (xElement: XNode) name =
    match xElement.XPathSelectElement("//project:" + name, projectNamespaceManager) with
    | null -> None
    | xe -> Some (xe)

let getProjectElementsByName (xElement: XNode) name =
    match xElement.XPathSelectElements("//project:" + name, projectNamespaceManager) with
    | null -> None
    | xe -> Some (xe)