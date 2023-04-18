module Narration.ApplicationError

open System

exception EnvarNotFound of varName:String
exception EnvarNotInvalidValue of varName:String * value:String
exception EnvarGetNotSupported

let printError(e: Exception): string = 
    match e with
        | EnvarNotFound name -> sprintf "Could not find environement variable %s (EnvarNotFound)" name
        | EnvarNotInvalidValue(name, value) -> sprintf "Invalid value in environment variable %s %s (EnvarNotInvalidValue)" name value
        | EnvarGetNotSupported -> "EnvarGetNotSupported"