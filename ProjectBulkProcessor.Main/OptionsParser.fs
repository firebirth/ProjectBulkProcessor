module OptionsParser

open Microsoft.CodeAnalysis.CSharp.Syntax
open System.Xml.Linq
open Microsoft.CodeAnalysis
open Scanner

type Options = { 
    targetFramework: string;
    isExecutable: bool;
    copyright: string option;
    company: string option;
    authors: string option;
    description: string option;
    version: string option;
    product: string option;
 }
 with defaultOptions = { targetFramework="net472";
    isExecutable=false;
    copyright=None;
    company=None;
    authors=None;
    description=None;
    version=None;
    product=None
 }

let private getAttributeList (assemblyInfo: SyntaxTree option) = 
    match assemblyInfo with
    | Some ai -> let root = ai.GetRoot() :?> CompilationUnitSyntax 
                 root.AttributeLists
                 |> Seq.collect (fun al -> al.Attributes)
                 |> Array.ofSeq
    | None -> Array.empty

let private buildAttributeValue (attribute: AttributeSyntax) = attribute.ArgumentList.Arguments |> Seq.map (fun a -> a.ToFullString().Trim '"') |> String.concat ","

let private buildAttributeOptions opts attributes =
    let folder optionState (attribute: AttributeSyntax) =
        match attribute.Name with
        | :? IdentifierNameSyntax as ins -> match ins.Identifier.Text with
                                            | "AssemblyCompany" -> { optionState with company = Some (buildAttributeValue attribute) }
                                            | "AssemblyCopyright" -> { optionState with copyright = Some (buildAttributeValue attribute) }
                                            | "AssemblyDescription" -> { optionState with description = Some (buildAttributeValue attribute) }
                                            | "AssemblyProduct" -> { optionState with product = Some (buildAttributeValue attribute) }
                                            | "AssemblyVersion" -> { optionState with version = Some (buildAttributeValue attribute) }
                                            | _ -> optionState
        | _ -> optionState

    Array.fold folder opts attributes

let private buildCsprojOptions  (xdoc: XDocument) opts =
    let newFramework = match XmlHelpers.getProjectElementByName xdoc "TargetFrameworkVersion" with
                       | None -> "net471"
                       | Some xml -> match xml.Value with
                                     | "v4.6.2" -> "net462"
                                     | _ -> "net471"
    let outputType = match XmlHelpers.getProjectElementByName xdoc "OutputType" with
                     | None -> false
                     | Some xml -> match xml.Value with
                                   | "Exe" -> true
                                   | _ -> false
    { opts with targetFramework = newFramework; isExecutable = outputType }

let private projectInfoMapper projectInfo =
    Options.defaultOptions
    |> buildCsprojOptions projectInfo.project
    |> buildAttributeOptions <| getAttributeList projectInfo.assemblyInfo

let buildProjectOptions projectInfos = Array.map projectInfoMapper projectInfos
