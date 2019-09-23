module OptionsParser

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp.Syntax
open System.Xml.Linq

type UpgradeOptions =
    { targetFramework : string
      outputType : string option
      copyright : string option
      company : string option
      authors : string option
      description : string option
      version : string option
      product : string option }
    static member defaultOptions =
        { targetFramework = "net472"
          outputType = None
          copyright = None
          company = None
          authors = None
          description = None
          version = None
          product = None }

let private getAttributeList (assemblyInfo : SyntaxTree option) =
    match assemblyInfo with
    | Some ai ->
        let root = ai.GetRoot() :?> CompilationUnitSyntax
        root.AttributeLists |> List.ofSeq |> List.collect (fun al -> al.Attributes |> List.ofSeq)
    | None -> List.empty

let private buildAttributeValue (attribute : AttributeSyntax) =
    attribute.ArgumentList.Arguments
    |> List.ofSeq
    |> List.map (fun a -> a.ToFullString().Trim '"')
    |> String.concat ","

let private buildAttributeOptions opts attributes =
    let optionBuilder optionState (attribute : AttributeSyntax) =
        match attribute.Name with
        | :? IdentifierNameSyntax as ins ->
            match ins.Identifier.Text with
            | "AssemblyCompany" -> { optionState with company = Some(buildAttributeValue attribute) }
            | "AssemblyCopyright" -> { optionState with copyright = Some(buildAttributeValue attribute) }
            | "AssemblyDescription" -> { optionState with description = Some(buildAttributeValue attribute) }
            | "AssemblyProduct" -> { optionState with product = Some(buildAttributeValue attribute) }
            | "AssemblyVersion" -> { optionState with version = Some(buildAttributeValue attribute) }
            | _ -> optionState
        | _ -> optionState
    List.fold optionBuilder opts attributes

let private buildCsprojOptions opts (xdoc : XDocument) =
    let newFramework =
        match XmlHelpers.getProjectElementByName "TargetFrameworkVersion" xdoc with
        | None -> "net471"
        | Some xml ->
            match xml.Value with
            | "v4.6.1" -> "net461"
            | "v4.6.2" -> "net462"
            | _ -> "net471"

    let outputType = XmlHelpers.getProjectElementByName "OutputType" xdoc |> Option.map (fun x -> x.Value)
    { opts with targetFramework = newFramework
                outputType = outputType }

let buildProjectOptions : SyntaxTree option -> XDocument -> UpgradeOptions =
    getAttributeList
    >> buildAttributeOptions UpgradeOptions.defaultOptions
    >> buildCsprojOptions
