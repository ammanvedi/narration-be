open FSharpPlus
open System
open FsConfig
open dotenv.net

DotEnv.Load()

type EnvConfig = {
    GoogleApplicationCredentials: String
    AssemblyAiEndpoint: String
    AssemblyAiApiKey: String
}

type ApplicationError = 
    | EnvarNotFound of varName:String
    | EnvarNotInvalidValue of varName:String * value:String
    | EnvarGetNotSupported

let printError(e: ApplicationError): string = 
    match e with
        | EnvarNotFound name -> sprintf "Could not find environement variable %s (EnvarNotFound)" name
        | EnvarNotInvalidValue(name, value) -> sprintf "Invalid value in environment variable %s %s (EnvarNotInvalidValue)" name value
        | EnvarGetNotSupported -> "EnvarGetNotSupported"

let loadConfig(): Result<EnvConfig, ApplicationError> =
    match EnvConfig.Get<EnvConfig>() with
        | Ok config -> Ok config
        | Error error -> 
            match error with
            | NotFound envVarName -> 
                Error (EnvarNotFound envVarName)
            | BadValue (envVarName, value) ->
                Error (EnvarNotInvalidValue(envVarName, value))
            | NotSupported msg -> 
                Error EnvarGetNotSupported

let init(): Result<Unit, ApplicationError> = monad' {
    let! config = loadConfig()
    printfn "cfg %A" config
    return ()
}

[<EntryPoint>]
let main args =
    match init() with
        | Ok _ -> 0
        | Error e -> printError e |> failwith