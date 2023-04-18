module Narration.FileStore

open Google.Apis.Auth.OAuth2
open Google.Cloud.Storage.V1
open System
open FSharpPlus
open System.IO

type FileUrl = String

type FileStoreController =
    abstract uploadFile: filePath: String -> fileName: String -> fileType: String -> Async<string>

type ErrorMapper = int -> string

let asyncChoiceToResult<'T1, 'T2 when 'T2 :> Exception> (asyncChoice: Async<Choice<'T1, 'T2>>): Async<Result<'T1, 'T2>> =
    Async.map (
        fun c -> match c with
                    | Choice1Of2 result -> Ok result
                    | Choice2Of2 e -> Error e
    ) asyncChoice

type GoogleBucketFileStore(bucketName: String, credentialsLocation: String) =
    member this.client = StorageClient.Create()
    member this.credential = GoogleCredential.FromFile(credentialsLocation)
    member this.bucketName = bucketName
    
    interface FileStoreController with
        member this.uploadFile filePath fileName fileType = async {
            let fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read)
            let! object = this.client.UploadObjectAsync(this.bucketName, fileName, fileType,  fileStream)
                          |> Async.AwaitTask
            let signer = UrlSigner.FromCredential(this.credential)
            let! url = signer.SignAsync(this.bucketName, fileName, TimeSpan.FromHours(1)) |> Async.AwaitTask
            return url
        }


