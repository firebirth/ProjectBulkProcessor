module ProjectScanner

open System.IO
open System.Xml.Linq
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis
open OptionsParser
open DependencyParser
open ReferenceParser

type private ParsedProjectFiles = {
    project: XDocument;
    packages: XDocument option;
    assemblyInfo: SyntaxTree option;
}

type ProjectInfo = {
    options: Options;
    dependencies: Dependency[] option;
    references: Reference[] option;
}

let private findProjectFiles rootPath =
    let fileInfoMapper filePath = new FileInfo(filePath)
    Directory.GetFiles(rootPath, "*.csproj", SearchOption.AllDirectories)
    |> Array.map fileInfoMapper

let private buildSyntaxTree filename =
    use stream = File.OpenText filename
    stream.ReadToEnd()
    |> CSharpSyntaxTree.ParseText

let private readXml filePath =
    using (File.OpenRead filePath) XDocument.Load

let private findProjectRelatedFile filename (projectFile: FileInfo)  =
    Directory.GetFiles(projectFile.DirectoryName, filename, SearchOption.AllDirectories)
    |> Array.tryFind (fun _ -> true)

let private readProjectFiles (projectFile: FileInfo) =
    { 
        project = readXml projectFile.FullName;
        packages = findProjectRelatedFile "packages.config" projectFile |> Option.map readXml;
        assemblyInfo = findProjectRelatedFile "AssemblyInfo.cs" projectFile |> Option.map buildSyntaxTree
    }

let private buildProjectInfo projectFiles =
    {
        options = OptionsParser.buildProjectOptions projectFiles.assemblyInfo projectFiles.project;
        dependencies = DependencyParser.findPackageElements projectFiles.packages;
        references = ReferenceParser.findProjectReferences projectFiles.project
    }

let getProjectInfos rootPath = 
    rootPath
    |> findProjectFiles
    |> Array.map readProjectFiles
    |> Array.map buildProjectInfo
