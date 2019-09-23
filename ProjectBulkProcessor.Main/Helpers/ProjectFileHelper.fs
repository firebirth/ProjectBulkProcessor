module ProjectFileHelper

open System.IO

let findProjectFiles rootPath = Directory.GetFiles(rootPath, "*.csproj", SearchOption.AllDirectories) |> List.ofArray |> List.map FileInfo
