module OptionHelper

let filterNones col = col |> (Seq.filter Option.isSome >> Seq.map Option.get)
