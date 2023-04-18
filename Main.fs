open FSharpPlus
open System
open FsConfig
open dotenv.net
open Narration.ApplicationError
open Narration.FileStore

DotEnv.Load()

type EnvConfig = {
    GoogleApplicationCredentials: String
    GoogleStorageBucketName: String
    AssemblyAiEndpoint: String
    AssemblyAiApiKey: String
}

let loadConfig(): Result<EnvConfig, Exception> =
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

let init(): Async<String> = monad' {
    let! config = match loadConfig() with
                    | Ok config -> applicative.Return config
                    | Error e -> raise e
    let! fileStore =
        new GoogleBucketFileStore(
                config.GoogleStorageBucketName,
                config.GoogleApplicationCredentials
            ) :> FileStoreController |> applicative.Return
    return! fileStore.uploadFile "./scratch.txt" "test.txt" "text"
}

[<EntryPoint>]
let main args =
    match Async.RunSynchronously (init() |> Async.Catch) with
        | Choice1Of2 _ -> 0
        | Choice2Of2 e -> printError e |> failwith