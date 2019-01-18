module OptionHelper

let filterNones<'a> : ('a option seq -> 'a seq) =
    Seq.filter Option.isSome
    >> Seq.map Option.get
