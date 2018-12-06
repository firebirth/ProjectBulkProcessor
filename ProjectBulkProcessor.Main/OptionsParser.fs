module OptionsParser

open System.IO
open System.Collections.Generic
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open System.Xml.Linq

type Options = { targetFramework:string; isExecutable:bool; copyright: string; company: string; authors: string; description: string; version: string; product: string }

let private findAssemblyInfoFiles projectDir = Directory.GetFiles(projectDir, "AssemblyInfo.cs", SearchOption.AllDirectories)

let private getAttributeList assemblyInfoFile = 
    using (File.OpenText assemblyInfoFile) (fun fs -> fs.ReadToEnd())
    |> CSharpSyntaxTree.ParseText
    |> fun tree -> tree.GetRoot() :?> CompilationUnitSyntax
    |> fun cus -> cus.AttributeLists

let private buildAttributeOptions opts (attributeList: IEnumerable<AttributeListSyntax>) =
    let buildAttributeValue (attribute: AttributeSyntax) = attribute.ArgumentList.Arguments |> Seq.map (fun a -> a.ToFullString().Trim '"') |> String.concat ","
    let mutable options = opts
    for attribute in attributeList |> Seq.collect (fun attributeList -> attributeList.Attributes) do
        match attribute.Name with
        | :? IdentifierNameSyntax as ins -> match ins.Identifier.Text with
                                            | "AssemblyCompany" -> options <- { options with company = buildAttributeValue attribute }
                                            | "AssemblyCopyright" -> options <- { options with copyright = buildAttributeValue attribute }
                                            | "AssemblyDescription" -> options <- { options with description = buildAttributeValue attribute }
                                            | "AssemblyProduct" -> options <- { options with product = buildAttributeValue attribute }
                                            | "AssemblyVersion" -> options <- { options with version = buildAttributeValue attribute }
                                            | _ -> ()
        | _ -> ()
    options

let private buildAssemblyInfoOptions opts projectDir =
    projectDir
    |> findAssemblyInfoFiles
    |> Seq.exactlyOne
    |> getAttributeList
    |> buildAttributeOptions opts

let private readProjectFile projectDir =
    Directory.GetFiles(projectDir, "*.csproj")
    |> Seq.exactlyOne
    |> fun projFilePath -> using (File.OpenRead projFilePath) XDocument.Load

let private buildCsprojOptions opts (xdoc: XDocument) =
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

let buildProjectOptions projectDir =
    let defaultOptions = Unchecked.defaultof<Options>
    readProjectFile projectDir
    |> buildCsprojOptions defaultOptions
    |> buildAssemblyInfoOptions <| projectDir
