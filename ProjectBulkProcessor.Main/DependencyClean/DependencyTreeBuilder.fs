module DependencyTreeBuilder

open DependencyParser
open System.IO

type DependencyTree =
    | Leaf of Dependency
    | Node of Dependency * DependencyTree seq

let private dependencyParser (fi : FileInfo) =
    let xml = XmlHelpers.readXml fi.FullName
    (DependencyParser.findPackageElements xml, ReferenceParser.findProjectReferences xml, fi)

let buildTree = ProjectFileHelper.findProjectFiles >> Seq.map dependencyParser
