module DependencyTreeBuilder

open DependencyParser
open System.IO
open ReferenceParser

type DependencyTree<'a> =
    | Leaf of 'a
    | Node of 'a * DependencyTree<'a> list

type Project =
    { dependecies: Dependency list
      references: Reference list
      file: FileInfo }

let private dependencyParser (fi : FileInfo) =
    let xml = XmlHelpers.readXml fi.FullName
    { dependecies = DependencyParser.findPackageElements xml |> List.ofSeq
      references = ReferenceParser.findProjectReferences xml |> List.ofSeq
      file = fi }

let buildTree root =
    let projects = (ProjectFileHelper.findProjectFiles >> List.map dependencyParser) root
    let root = List.find (fun p -> p.references.Length = 0) projects
    root

