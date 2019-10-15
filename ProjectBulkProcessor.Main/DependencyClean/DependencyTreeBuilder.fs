module DependencyTreeBuilder

open DependencyParser
open System.IO
open ReferenceParser

type Tree<'a> =
    | Leaf of 'a
    | Node of 'a * Tree<'a> list



let private dependencyParser (fi : FileInfo) =
    let xml = XmlHelpers.readXml fi.FullName
    { dependecies = DependencyParser.findPackageElements xml |> List.ofSeq
      references = ReferenceParser.findProjectReferences xml |> List.ofSeq
      file = fi }

let buildTree root =
    let projects = (ProjectFileHelper.findProjectFiles >> List.map dependencyParser) root
    let roots = List.filter (fun p -> p.references.Length = 0) projects
    let getSubProjects root projects =
        List.filter (fun p -> List.contains root p.references)
    let rec addToTree (tree: Tree<Project>) (subProjects: Project list) : Tree<Project> =
        match tree with
        | Leaf p -> Node (p, List.filter (fun sp -> List.contains p.file.FullName sp.references) |> List.map (fun s -> Leaf s))
        | Node (l, n) -> Node (l, List.map (fun e -> addToTree e subProjects) n)
    true
