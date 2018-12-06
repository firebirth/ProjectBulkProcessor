module Parser

open System.IO
open System.Collections.Generic
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

type Options = {targetFramework:string; isExecutable:bool; copyright: string; company: string; authors: string; description: string; version: string; product: string }

let private oldNewFrameworkAliasMap = dict[
                                            "v4.6.2", "net462";
                                            "v4.6.1", "net461";
                                          ]

let private findAssemblyInfoFiles projectPath = Directory.GetFiles(projectPath, "AssemblyInfo.cs", SearchOption.AllDirectories)

let private getAttributeList assemblyInfoFile = 
        use fs = File.OpenText assemblyInfoFile
        let tree = fs.ReadToEnd() |> CSharpSyntaxTree.ParseText
        let cus = tree.GetRoot() :?> CompilationUnitSyntax
        cus.AttributeLists

let private buildAttributeOptions (attributeList: IEnumerable<AttributeListSyntax>) =
    let buildAttributeValue (attribute: AttributeSyntax) = attribute.ArgumentList.Arguments |> Seq.map (fun a -> a.ToFullString().Trim '"') |> String.concat ","
    let mutable options = {targetFramework = ""; isExecutable= false; copyright= ""; company= ""; authors= ""; description= ""; version= ""; product= ""}
    let allAttributes = attributeList |> Seq.collect (fun attributeList -> attributeList.Attributes)
    for attribute in allAttributes do
        match attribute.Name with
        | :? IdentifierNameSyntax as ins when ins.Identifier.Text = "AssemblyCompany" -> options <- {options with company = buildAttributeValue attribute }
        | :? IdentifierNameSyntax as ins when ins.Identifier.Text = "AssemblyCopyright" -> options <- {options with copyright = buildAttributeValue attribute }
        | :? IdentifierNameSyntax as ins when ins.Identifier.Text = "AssemblyDescription" -> options <- {options with description = buildAttributeValue attribute }
        | :? IdentifierNameSyntax as ins when ins.Identifier.Text = "AssemblyProduct" -> options <- {options with product = buildAttributeValue attribute }
        | :? IdentifierNameSyntax as ins when ins.Identifier.Text = "AssemblyVersion" -> options <- {options with version = buildAttributeValue attribute }
        | _ -> ()
    options

let private buildAssemblyInfoOptions projectPath =
    projectPath
    |> findAssemblyInfoFiles
    |> Seq.exactlyOne
    |> getAttributeList
    |> buildAttributeOptions
