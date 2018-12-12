module OptionHelper

let filterNones arr =
    let arraySomeSelector a = a |> Array.filter Option.isSome |> Array.map (fun e -> e.Value)
    Option.map arraySomeSelector arr
