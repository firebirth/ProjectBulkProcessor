module Scanner

open System.IO

type private pathValidationResult = { isValid: bool }

let private validateDirectory rootPath = { isValid = Directory.Exists(rootPath)  }

let private getFileList rootPath = Directory.GetFiles(rootPath, "*.csproj", SearchOption.AllDirectories)

let private matchValidationResult validationResult = 
    match validationResult with
    | { isValid = true } -> getFileList
    | _ -> fun _ -> Array.empty

let getProjectFiles rootPath = 
    rootPath
    |> validateDirectory
    |> matchValidationResult