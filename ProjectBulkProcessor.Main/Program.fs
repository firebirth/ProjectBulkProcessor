// Learn more about F# at http://fsharp.org
open System

[<EntryPoint>]
let main argv =
    let a = UpgradeProcessor.upgradeProjects "C:\\Repos\\OrderService"
    printfn "Hello World from F#!"
    0 // return an integer exit code
