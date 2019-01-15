module ProjectBuilder

open DependencyParser
open ReferenceParser
open OptionsParser
open ProjectScanner
open System.Xml.Linq

let private XAttribute name value = XAttribute (XName.Get name, value)
let private XElement<'a> name (value: 'a when 'a :> obj) = XElement (XName.Get name, value)
let private XDocument<'a> (content: 'a seq when 'a :> obj) = XDocument content

let private mapDependencyToElements dependencies =
    let mapper dependecy =
        let attributes = [| XAttribute "Include" dependecy.packageId; XAttribute "Version" dependecy.version |]
        XElement "PackageReference" attributes
    Seq.map mapper dependencies

let private mapReferenceToElements references =
    let mapper reference =
        let attributes = XAttribute "Include" reference.relativePath |> Seq.singleton
        XElement "ProjectReference" attributes
    Seq.map mapper references
    
let private mapOptionsToElements options =
    let mapElement elementName elementValue = Option.map (XElement elementName) elementValue
    [
        Some (XElement "TargetFramework" options.targetFramework);
        mapElement "OutputType" options.outputType;
        mapElement "Copyright" options.copyright;
        mapElement "Company" options.outputType;
        mapElement "Authors" options.authors;
        mapElement "Description" options.description;
        mapElement "Version" options.version;
        mapElement "Product" options.product;
    ]
    |> OptionHelper.filterNones

let private buildElements projectInfo =
    let itemGroup content = XElement "ItemGroup" content
    let propertyGroup content = XElement "PropertyGroup" content
    [
        mapOptionsToElements projectInfo.options |> propertyGroup;
        mapDependencyToElements projectInfo.dependencies |> itemGroup;
        mapReferenceToElements projectInfo.references |> itemGroup;
    ]

let buildProject projectInfo =
    let xdoc = projectInfo 
               |> buildElements 
               |> Seq.map (fun e -> e :> XObject)
               |> Seq.append [| XAttribute "Sdk" "Microsoft.NET.Sdk" |] 
               |> XElement "Project"
               |> Seq.singleton
               |> XDocument
    (xdoc, projectInfo)