module OptionsParser

open Microsoft.CodeAnalysis.CSharp.Syntax
open System.Xml.Linq
open Microsoft.CodeAnalysis

type Options = { 
    targetFramework: string;
    outputType: string option;
    copyright: string option;
    company: string option;
    authors: string option;
    description: string option;
    version: string option;
    product: string option;
 }
 with static member defaultOptions = {
        targetFramework = "net472";
        outputType = None;
        copyright = None;
        company = None;
        authors = None;
        description = None;
        version = None;
        product = None;
}

let private getAttributeList (assemblyInfo: SyntaxTree option) =
    match assemblyInfo with
    | Some ai -> let root = ai.GetRoot() :?> CompilationUnitSyntax 
                 root.AttributeLists
                 |> Seq.collect (fun al -> al.Attributes)
                 |> Array.ofSeq
    | None -> Array.empty

let private buildAttributeValue (attribute: AttributeSyntax) =
    attribute.ArgumentList.Arguments
    |> Seq.map (fun a -> a.ToFullString().Trim '"')
    |> String.concat ","

let private buildAttributeOptions opts attributes =
    let optionBuilder optionState (attribute: AttributeSyntax) =
        match attribute.Name with
        | :? IdentifierNameSyntax as ins -> match ins.Identifier.Text with
                                            | "AssemblyCompany" -> { optionState with company = Some (buildAttributeValue attribute) }
                                            | "AssemblyCopyright" -> { optionState with copyright = Some (buildAttributeValue attribute) }
                                            | "AssemblyDescription" -> { optionState with description = Some (buildAttributeValue attribute) }
                                            | "AssemblyProduct" -> { optionState with product = Some (buildAttributeValue attribute) }
                                            | "AssemblyVersion" -> { optionState with version = Some (buildAttributeValue attribute) }
                                            | _ -> optionState
        | _ -> optionState

    Array.fold optionBuilder opts attributes

let private buildCsprojOptions  (xdoc: XDocument) opts =
    let newFramework = match XmlHelpers.getProjectElementByName xdoc "TargetFrameworkVersion" with
                       | None -> "net471"
                       | Some xml -> match xml.Value with
                                     | "v4.6.1" -> "net461"
                                     | "v4.6.2" -> "net462"
                                     | _ -> "net471"
    let outputType = XmlHelpers.getProjectElementByName xdoc "OutputType"
                     |> Option.map (fun x -> x.Value)
    { opts with targetFramework = newFramework; outputType = outputType }

let buildProjectOptions assemblyInfo project =
    Options.defaultOptions
    |> buildAttributeOptions <| getAttributeList assemblyInfo
    |> buildCsprojOptions project
