module ProjectFileHelper

open System.IO

let findProjectFiles rootPath =
    let fileInfoMapper filePath = FileInfo filePath
    Directory.GetFiles(rootPath, "*.csproj", SearchOption.AllDirectories)
    |> Seq.map fileInfoMapper