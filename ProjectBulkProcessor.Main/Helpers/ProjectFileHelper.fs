module ProjectFileHelper

open System.IO

let findProjectFiles rootPath = Directory.GetFiles(rootPath, "*.csproj", SearchOption.AllDirectories) |> Seq.map FileInfo
