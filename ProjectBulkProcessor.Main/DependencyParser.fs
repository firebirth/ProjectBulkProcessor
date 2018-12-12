module DependencyParser

open System.Xml.Linq

type Dependency = { packageId: string; version: string; }

let private elementSelector (xElement:XElement) =
    let id = XmlHelpers.getAttributeValue xElement "id"
    let version = XmlHelpers.getAttributeValue xElement "version"
    match (id, version) with
    | (Some id, Some version) -> Some ({ packageId = id; version = version })
    | _ -> None

let findPackageElements xdoc =
    XmlHelpers.mapElements xdoc "//packages" elementSelector
    |> OptionHelper.filterNones
