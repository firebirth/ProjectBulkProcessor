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
    filesToRemove: string seq;
}

type ProjectUpgradeInfo = {
    projectPath: string;
    options: Options;
    dependencies: Dependency seq;
    references: Reference seq;
    filesToRemove: string seq;
}

let private buildSyntaxTree filename =
    use stream = File.OpenText filename
    stream.ReadToEnd()
    |> CSharpSyntaxTree.ParseText

let private findProjectRelatedFile filename (projectFile: FileInfo)  =
    Directory.GetFiles(projectFile.DirectoryName, filename, SearchOption.AllDirectories)
    |> Seq.tryFind (fun _ -> true)

let private readProjectFiles (projectFile: FileInfo) =
    let packages = findProjectRelatedFile "packages.config" projectFile
    let assemblyInfo = findProjectRelatedFile "AssemblyInfo.cs" projectFile
    {
        projectPath = projectFile.FullName;
        project = XmlHelpers.readXml projectFile.FullName;
        packages = Option.map XmlHelpers.readXml packages;
        assemblyInfo = Option.map buildSyntaxTree assemblyInfo;
        filesToRemove = OptionHelper.filterNones [ packages; assemblyInfo ]
    }

let private buildProjectInfo projectFile =
    {
        options = OptionsParser.buildProjectOptions projectFile.assemblyInfo projectFile.project;
        dependencies = match projectFile.packages with
                       | Some p -> DependencyParser.findPackageElements p
                       | None -> Seq.empty
        references = ReferenceParser.findProjectReferences projectFile.project;
        projectPath = projectFile.projectPath;
        filesToRemove = projectFile.filesToRemove;
    }

let getProjectInfos = ProjectFileHelper.findProjectFiles >> Seq.map (readProjectFiles >> buildProjectInfo)
