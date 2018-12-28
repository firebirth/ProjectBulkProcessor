module OptionHelper

let filterNones arr = arr |> Array.filter Option.isSome |> Array.map Option.get
