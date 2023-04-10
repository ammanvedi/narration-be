open FSharpPlus
open System.Net

let uploadFile(path: string)

type Err = string

let succ: Result<string, Err> = Ok "abc"

let pureFunc(s: string) = s 

let fail(arg: string): Result<string, Err> = Error "sds"

let maybeWithSideFx = monad' { 
    let! p = succ
    let! q = fail p
    let! r  =  applicative.Return (pureFunc q)
    return r
}

[<EntryPoint>]
let main args =
    match maybeWithSideFx with
        | Ok x -> printfn "OK : %s" x
        | Error s -> printfn "Error : %s" s
    0