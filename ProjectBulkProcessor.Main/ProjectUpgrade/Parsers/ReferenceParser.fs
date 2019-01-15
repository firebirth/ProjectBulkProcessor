module ReferenceParser

open System.Xml.Linq

type Reference = { relativePath: string; }

let findProjectReferences xdoc = 
    let elementSelector xElement =
        XmlHelpers.getAttributeValue xElement "Include"
        |> Option.map (fun p -> { relativePath = p })
    XmlHelpers.mapProjectElements elementSelector xdoc "ProjectReference"
    |> OptionHelper.filterNones
