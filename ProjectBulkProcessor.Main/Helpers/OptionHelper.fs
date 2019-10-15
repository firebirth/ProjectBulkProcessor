module OptionHelper

let filterNones<'a> : ('a option list -> 'a list) = List.filter Option.isSome >> List.map Option.get
