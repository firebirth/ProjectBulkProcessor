module OptionHelper

let filterNones col = (Seq.filter Option.isSome >> Seq.map Option.get) col
