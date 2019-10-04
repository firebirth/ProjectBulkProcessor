module ReferenceParser

open System.Xml.Linq
open DependencyParser
open System.IO



let findProjectReferences : XNode -> Reference list =
    let elementSelector = XmlHelpers.getAttributeValue "Include" >> Option.map (fun p -> { relativePath = p })
    XmlHelpers.mapProjectElements elementSelector "ProjectReference" >> OptionHelper.filterNones
