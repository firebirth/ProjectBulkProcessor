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
    product: string option
 }

let private getAttributeList (assemblyInfo: SyntaxTree option) = 
    match assemblyInfo with
    | Some ai -> ai.GetRoot() :?> CompilationUnitSyntax 
                 |> fun cus -> cus.AttributeLists
                 |> Seq.collect (fun a -> a.Attributes)
    | None -> Seq.empty

let private buildAttributeOptions opts (attributeList: AttributeSyntax seq)  =
    let buildAttributeValue (attribute: AttributeSyntax) = attribute.ArgumentList.Arguments |> Seq.map (fun a -> a.ToFullString().Trim '"') |> String.concat ","
    let mutable options = opts
    for attribute in attributeList do
        match attribute.Name with
        | :? IdentifierNameSyntax as ins -> match ins.Identifier.Text with
                                            | "AssemblyCompany" -> options <- { options with company = Some (buildAttributeValue attribute) }
                                            | "AssemblyCopyright" -> options <- { options with copyright = Some (buildAttributeValue attribute) }
                                            | "AssemblyDescription" -> options <- { options with description = Some (buildAttributeValue attribute) }
                                            | "AssemblyProduct" -> options <- { options with product = Some (buildAttributeValue attribute) }
                                            | "AssemblyVersion" -> options <- { options with version = Some (buildAttributeValue attribute) }
                                            | _ -> ()
        | _ -> ()
    options

let private buildCsprojOptions  (xdoc: XDocument) opts =
    let oldFramework = XmlHelpers.getProjectElementByName xdoc "TargetFrameworkVersion"
    let newFramework = match oldFramework with
                       | null -> "net471"
                       | _ -> match oldFramework.Value with
                              | "v4.6.2" -> "net462"
                              | _ -> "net471"
    let outputTypeElement = XmlHelpers.getProjectElementByName xdoc "OutputType"
    let outputType = match outputTypeElement with
                     | null -> false
                     | _ -> match outputTypeElement.Value with
                            | "Exe" -> true
                            | _ -> false
    { opts with targetFramework = newFramework; isExecutable = outputType }

let buildProjectOptions (projectInfos: ProjectInfo seq) =
    seq {
        for projectInfo in projectInfos do
            yield Unchecked.defaultof<Options>
                  |> buildCsprojOptions projectInfo.project
                  |> buildAttributeOptions <| getAttributeList projectInfo.assemblyInfo
    }
