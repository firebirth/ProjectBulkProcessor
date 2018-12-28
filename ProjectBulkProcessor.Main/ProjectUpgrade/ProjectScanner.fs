module ProjectScanner

open System.IO
open System.Xml.Linq
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis
open OptionsParser
open DependencyParser
open ReferenceParser

type private ParsedProjectFile = {
    projectPath: string;
    project: XDocument;
    packages: XDocument option;
    assemblyInfo: SyntaxTree option;
    filesToRemove: string[];
}

type ProjectInfo = {
    projectPath: string;
    options: Options;
    dependencies: Dependency array option;
    references: Reference array option;
    filesToRemove: string array;
}

let private findProjectFiles rootPath =
    let fileInfoMapper filePath = FileInfo filePath
    Directory.GetFiles(rootPath, "*.csproj", SearchOption.AllDirectories)
    |> Seq.map fileInfoMapper

let private buildSyntaxTree filename =
    use stream = File.OpenText filename
    stream.ReadToEnd()
    |> CSharpSyntaxTree.ParseText

let private readXml filePath =
    using (File.OpenRead filePath) XDocument.Load

let private findProjectRelatedFile filename (projectFile: FileInfo)  =
    Directory.GetFiles(projectFile.DirectoryName, filename, SearchOption.AllDirectories)
    |> Seq.tryFind (fun _ -> true)

let private readProjectFiles (projectFile: FileInfo) =
    let packages = findProjectRelatedFile "packages.config" projectFile
    let assemblyInfo = findProjectRelatedFile "AssemblyInfo.cs" projectFile
    {
        projectPath = projectFile.FullName;
        project = readXml projectFile.FullName;
        packages = packages |> Option.map readXml;
        assemblyInfo = assemblyInfo |> Option.map buildSyntaxTree;
        filesToRemove = [ packages; assemblyInfo ] |> OptionHelper.filterNones |> Array.ofSeq
    }

let private buildProjectInfo projectFile =
    {
        options = OptionsParser.buildProjectOptions projectFile.assemblyInfo projectFile.project;
        dependencies = DependencyParser.findPackageElements projectFile.packages |> Option.map Array.ofSeq;
        references = ReferenceParser.findProjectReferences projectFile.project |> Option.map Array.ofSeq;
        projectPath = projectFile.projectPath;
        filesToRemove = projectFile.filesToRemove;
    }

let getProjectInfos rootPath = 
    rootPath
    |> findProjectFiles
    |> Seq.map (readProjectFiles >> buildProjectInfo)
