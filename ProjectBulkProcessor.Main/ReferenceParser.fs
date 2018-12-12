module ReferenceParser

open System.Xml.Linq

type Reference = { relativePath: string; }

let private elementSelector (xElement: XElement) =
    let path = XmlHelpers.getAttributeValue xElement "Include"
    match path with
    | Some path -> Some ({ relativePath = path })
    | _ -> None

let findProjectReferences xdoc = 
    XmlHelpers.mapElements xdoc "ProjectReference" elementSelector 
    |> OptionHelper.filterNones