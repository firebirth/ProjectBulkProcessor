module NullHelper

let inline handle value =
    match value with
    | null -> None
    | value -> Some value
