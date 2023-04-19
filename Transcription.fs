module Narration.Transcription

open System

type TranscriptionResult = {
    x: String
}

type TranscriptionService =
    abstract getTranscriptionForAudio: publicAudioUrl: String -> Async<TranscriptionResult>