module FsLoggingTypes
open System
open Hopac
open Hopac.Infixes
open FsTimeTypes

// this should be it's own library
type LogLevel =
  | Debug
  | Verbose
  | Information
  | Warning
  | Exception
with override x.ToString() =
      match x with
      | Debug -> "DEBUG"
      | Verbose -> "VERBOSE"
      | Information -> "INFO"
      | Warning -> "WARN"
      | Exception -> "EXCEPT"

type Message =
  {
    LogLevel : LogLevel
    Message : string
    Tags : string list
    Fields : Map<string, obj>
    DateTime : DateTime'
  }
  static member Simple level message =
    DateTime'.Now()
    >>- fun x ->
      {
        LogLevel = level
        Message = message
        Tags = []
        Fields = Map.empty
        DateTime = x
      }
  static member AddField (key : string) (o : obj) (x : Message)=
    {
      LogLevel = x.LogLevel
      Message = x.Message
      Tags = x.Tags
      Fields = x.Fields |> Map.add key o
      DateTime = x.DateTime
    }
  static member AddFields (fields : Map<string, obj>) (x : Message) =
    let f1 = x.Fields |> Map.toList
    let f2 = fields |> Map.toList
    let f = f1 @ f2
    let m = f |> List.fold (fun m (s,o) -> m |> Map.add s o) Map.empty
    {
      LogLevel = x.LogLevel
      Message = x.Message
      Tags = x.Tags
      Fields = m
      DateTime = x.DateTime
    }

type Endpoint = Message -> Job<unit>

type Endpoints = | Endpoints' of Map<string, Endpoint>

// This is my logger in its simplest form,
// you build a logger, you add endpoints to it,
// then you log, its up to the endpoint wether it should
// log depending on the verbosity
// The logger is injected everywhere we intend to use it
type Logger =
  {
    Endpoints : Endpoints
  }
    static member Add (key : string) (func : Endpoint) (x : Logger) : Logger =
      let ep = x.Endpoints |> function | Endpoints' x -> x
      let next = ep |> Map.add key func
      let next' = next |> Endpoints'
      {
        Endpoints = next'
      }
    member x.Remove (key : string) : Logger =
      let ep = x.Endpoints |> function | Endpoints' x -> x
      let next = ep |> Map.remove key
      let next' = next |> Endpoints'
      {
        Endpoints = next'
      }
    member x.Log (msg : Message ) =
      let ep = x.Endpoints |> function | Endpoints' x -> x
      let funcs = ep |> Map.toList |> List.map snd
      funcs |> List.map (fun fn -> fn msg) |> Job.conIgnore

    static member New () : Logger =
        {
          Endpoints = Map.empty |> Endpoints'
        }
