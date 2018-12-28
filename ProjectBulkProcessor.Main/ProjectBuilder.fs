module ProjectBuilder

open DependencyParser
open ReferenceParser
open OptionsParser
open ProjectScanner
open System.Xml.Linq

let private XAttribute name value = new XAttribute(XName.Get name, value)
let private XElement<'a> name (value: 'a when 'a :> obj) = new XElement(XName.Get name, value)
let private XDocument<'a> (content: 'a array when 'a :> obj) = new XDocument(content)

let private mapDependencyToElements dependencies =
    let mapper dependecy =
        let attributes = [| XAttribute "Include" dependecy.packageId; XAttribute "Version" dependecy.version |]
        XElement "PackageReference" attributes
    Array.map mapper dependencies

let private mapReferenceToElements references =
    let mapper reference =
        let attributes = [| XAttribute "Include" reference.relativePath |]
        XElement "ProjectReference" attributes
    Array.map mapper references
    
let private mapOptionsToElements options =
    let mapElement elementName elementValue = Option.map (XElement elementName) elementValue
    [|
        Some (XElement "TargetFramework" options.targetFramework);
        mapElement "OutputType" options.outputType;
        mapElement "Copyright" options.copyright;
        mapElement "Company" options.outputType;
        mapElement "Authors" options.authors;
        mapElement "Description" options.description;
        mapElement "Version" options.version;
        mapElement "Product" options.product;
    |] |> OptionHelper.filterNones

let private buildElements projectInfo =
    let itemGroup content = XElement "ItemGroup" content
    let propertyGroup content = XElement "PropertyGroup" content
    [|
        mapOptionsToElements projectInfo.options |> propertyGroup |> Some;
        Option.map mapDependencyToElements projectInfo.dependencies |> Option.map itemGroup;
        Option.map mapReferenceToElements projectInfo.references |> Option.map itemGroup;
    |] |> OptionHelper.filterNones

let buildProject projectInfo =
    projectInfo 
    |> buildElements 
    |> Array.map (fun e -> e :> XObject)
    |> Array.append [| XAttribute "Sdk" "Microsoft.NET.Sdk" |] 
    |> XElement "Project"
    |> Array.create 1
    |> XDocument