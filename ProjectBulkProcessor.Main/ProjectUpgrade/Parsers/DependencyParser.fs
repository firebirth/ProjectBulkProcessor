module DependencyParser

open System.Xml.Linq

type Dependency = { packageId: string; version: string; }

let findPackageElements xdoc =
    let elementSelector (xElement:XElement) =
        let mapper id version = { packageId = id; version = version }
        let id = XmlHelpers.getAttributeValue xElement "id"
        let version = XmlHelpers.getAttributeValue xElement "version"
        Option.map2 mapper id version

    xdoc
    |> Option.map (XmlHelpers.mapElements elementSelector "//package")
    |> Option.flatten
