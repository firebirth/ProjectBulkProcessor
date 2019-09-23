module ReferenceParser

open System.Xml.Linq

type Reference =
    { relativePath : string }

let findProjectReferences : XNode -> Reference list =
    let elementSelector = XmlHelpers.getAttributeValue "Include" >> Option.map (fun p -> { relativePath = p })
    XmlHelpers.mapProjectElements elementSelector "ProjectReference" >> OptionHelper.filterNones
