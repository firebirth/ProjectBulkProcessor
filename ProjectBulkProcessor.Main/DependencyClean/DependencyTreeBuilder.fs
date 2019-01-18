module DependencyTreeBuilder

open DependencyParser
open System.IO
open ReferenceParser

type DependencyTree<'a> =
    | Leaf of 'a
    | Node of 'a * DependencyTree<'a> seq

type Project =
    { dependecies: Dependency array
      references: Reference array
      file: FileInfo }

let private dependencyParser (fi : FileInfo) =
    let xml = XmlHelpers.readXml fi.FullName
    { dependecies = DependencyParser.findPackageElements xml |> Array.ofSeq
      references = ReferenceParser.findProjectReferences xml |> Array.ofSeq
      file = fi }

let buildTree root =
    let projects = (ProjectFileHelper.findProjectFiles >> Seq.map dependencyParser) root
    let root = Seq.find (fun p -> p.references.Length = 0) projects

