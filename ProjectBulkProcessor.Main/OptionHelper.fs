module OptionHelper

let filterNones arr =
    match arr with
    | Some arr -> Some (arr |> Array.filter Option.isSome |> Array.map (fun e -> e.Value))
    | None -> None