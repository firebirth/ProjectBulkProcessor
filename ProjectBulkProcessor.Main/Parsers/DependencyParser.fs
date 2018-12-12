module DependencyParser

open System.Xml.Linq

type Dependency = { packageId: string; version: string; }

let private elementSelector (xElement:XElement) =
    let mapper id version = { packageId = id; version = version }
    let id = XmlHelpers.getAttributeValue xElement "id"
    let version = XmlHelpers.getAttributeValue xElement "version"
    Option.map2 mapper id version

let findPackageElements xdoc =
    match xdoc with
    | Some doc -> XmlHelpers.mapElements doc "//package" elementSelector |> OptionHelper.filterNones
    | None -> None
