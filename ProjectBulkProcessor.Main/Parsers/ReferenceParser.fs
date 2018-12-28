module ReferenceParser

open System.Xml.Linq

type Reference = { relativePath: string; }

let private elementSelector (xElement: XElement) =
    XmlHelpers.getAttributeValue xElement "Include"
    |> Option.map (fun p -> { relativePath = p })

let findProjectReferences xdoc = 
    XmlHelpers.mapProjectElements xdoc "ProjectReference" elementSelector 
