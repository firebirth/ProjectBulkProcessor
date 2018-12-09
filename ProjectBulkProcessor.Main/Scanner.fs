module Scanner

open System.IO
open System.Xml.Linq
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis

type ProjectInfo = {
    project: XDocument;
    packages: XDocument option;
    assemblyInfo: SyntaxTree option
}

let private findProjectFiles rootPath =
    Directory.GetFiles(rootPath, "*.csproj", SearchOption.AllDirectories)
    |> Seq.map (fun f -> new FileInfo(f))

let private buildSyntaxTree filename = 
    using (File.OpenText filename) (fun fs -> fs.ReadToEnd())
    |> CSharpSyntaxTree.ParseText

let private readXml filePath =
    using (File.OpenRead filePath) XDocument.Load

let private findProjectRelatedFile filename (projectFile: FileInfo)  =
    Directory.GetFiles(projectFile.DirectoryName, filename, SearchOption.AllDirectories)
    |> Array.tryFind (fun _ -> true)

let private buildProjectInfo (projectFile: FileInfo) =
    { 
        project = projectFile.FullName |> readXml;
        packages = projectFile |> findProjectRelatedFile "AssemblyInfo.cs" |> Option.map readXml;
        assemblyInfo = projectFile |> findProjectRelatedFile "packages.config" |> Option.map buildSyntaxTree
    }

let getProjectInfos rootPath = 
    rootPath
    |> findProjectFiles
    |> Seq.map buildProjectInfo
