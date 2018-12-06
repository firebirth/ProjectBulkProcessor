module Parser

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

let private buildAttributeOptions (attributeList: IEnumerable<AttributeListSyntax>) opts =
    let buildAttributeValue (attribute: AttributeSyntax) = attribute.ArgumentList.Arguments |> Seq.map (fun a -> a.ToFullString().Trim '"') |> String.concat ","

    let mutable options = opts
    for attribute in attributeList |> Seq.collect (fun attributeList -> attributeList.Attributes) do
        match attribute.Name with
        | :? IdentifierNameSyntax as ins when ins.Identifier.Text = "AssemblyCompany" -> options <- { options with company = buildAttributeValue attribute }
        | :? IdentifierNameSyntax as ins when ins.Identifier.Text = "AssemblyCopyright" -> options <- { options with copyright = buildAttributeValue attribute }
        | :? IdentifierNameSyntax as ins when ins.Identifier.Text = "AssemblyDescription" -> options <- { options with description = buildAttributeValue attribute }
        | :? IdentifierNameSyntax as ins when ins.Identifier.Text = "AssemblyProduct" -> options <- { options with product = buildAttributeValue attribute }
        | :? IdentifierNameSyntax as ins when ins.Identifier.Text = "AssemblyVersion" -> options <- { options with version = buildAttributeValue attribute }
        | _ -> ()
    options

let private buildAssemblyInfoOptions projectDir =
    projectDir
    |> findAssemblyInfoFiles
    |> Seq.exactlyOne
    |> getAttributeList
    |> buildAttributeOptions

let private readProjectFile projectDir =
    Directory.GetFiles(projectDir, "*.csproj")
    |> Seq.exactlyOne
    |> fun projFilePath -> using (File.OpenRead projFilePath) XDocument.Load

let private buildFrameworkOptions (xdoc: XDocument) opts =
    let mutable options = opts
    let oldFramework = XmlHelpers.getProjectElementByName xdoc "TargetFrameworkVersion"
    let a = match oldFramework with
        | null -> ()
        | _ -> _
    let newFramework = match oldFramework.Value with
                       | "v4.6.2" -> "net462"
                       | _ -> "net471"
    options <- {options with targetFramework = newFramework}
    options

let private buildOutputTypeOptions (xdoc: XDocument) opts =
    let outputTypeElement = XmlHelpers.getProjectElementByName xdoc "OutputType"
    let outputType = match outputTypeElement.Value with
        | 

let private a =
    readProjectFile ""
    |> buildFrameworkOptions
