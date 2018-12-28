// Learn more about F# at http://fsharp.org

open System

[<EntryPoint>]
let main argv =
    let a = ProjectScanner.getProjectInfos "C:\\Repos\\OrderService"
            |> Seq.map ProjectBuilder.buildProject
            |> Seq.iter (fun (p, i) -> p.Save(i.projectPath))
    printfn "Hello World from F#!"
    0 // return an integer exit code
