module DependencyTreeBuilder

open System.IO
open DependencyParser


type DependencyTree =
    | Leaf of Dependency
    | Node of Dependency * DependencyTree seq

let private dependencyParser (fi: FileInfo) =
    let parser = XmlHelpers.readXml >> DependencyParser.findPackageElements
    (parser fi.FullName, fi)
    

let buildTree root =
    ProjectFileHelper.findProjectFiles root
    |> Seq.map dependencyParser
