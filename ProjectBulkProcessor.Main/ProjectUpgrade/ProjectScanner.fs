module ProjectScanner

open DependencyParser
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open OptionsParser
open System.IO
open System.Xml.Linq

type private ParsedProjectFile =
    { projectPath : string
      project : XDocument
      packages : XDocument option
      assemblyInfo : SyntaxTree option
      filesToRemove : string list }

type Project =
    { projectPath : FileInfo
      dependencies : Dependency list
      references : Reference list }
and Reference = Project of Project

type ProjectUpgradeInfo =
    { project: Project
      options : UpgradeOptions
      filesToRemove : string list }

let private buildSyntaxTree filename =
    use stream = File.OpenText filename
    stream.ReadToEnd() |> CSharpSyntaxTree.ParseText

let private findProjectRelatedFile (projectFile : FileInfo) filename =
    Directory.GetFiles(projectFile.DirectoryName, filename, SearchOption.AllDirectories)
    |> Array.tryHead

let private readProjectFiles (projectFile : FileInfo) =
    let finder = findProjectRelatedFile projectFile
    let packages = finder "packages.config"
    let assemblyInfo = finder "AssemblyInfo.cs"
    { projectPath = projectFile.FullName
      project = XmlHelpers.readXml projectFile.FullName
      packages = Option.map XmlHelpers.readXml packages
      assemblyInfo = Option.map buildSyntaxTree assemblyInfo
      filesToRemove = OptionHelper.filterNones [ packages; assemblyInfo ] }

let private 

let private buildProjectInfo projectFile =
    { options = OptionsParser.buildProjectOptions projectFile.assemblyInfo projectFile.project
      dependencies = match projectFile.packages with
                     | Some p -> DependencyParser.findPackageElements p
                     | None -> List.empty
      references = ReferenceParser.findProjectReferences projectFile.project
      projectPath = projectFile.projectPath
      filesToRemove = projectFile.filesToRemove }

let getProjectInfos =
    ProjectFileHelper.findProjectFiles >> List.map (readProjectFiles >> buildProjectInfo)
