namespace BatailleNavale.Rest

[<AutoOpen>]
module RestFul =
    open Newtonsoft.Json
    open Newtonsoft.Json.Serialization
    open Suave
    open Suave.Operators
    open Suave.Successful
    open Suave.Filters

    type RestResource<'a> = {
        GetAll : unit -> 'a seq
    }

    let JSON v =
        let serializer = new JsonSerializerSettings()
        serializer.ContractResolver <- new CamelCasePropertyNamesContractResolver()
        JsonConvert.SerializeObject(v, serializer)
        |> OK
        >=> Writers.setMimeType "application/json; charset=utf-8"

    // string -> RestResource<'a> -> WebPart
    let rest resourceName resource =
        let resourcePath = "/" + resourceName
        let getAll = warbler (fun _ -> resource.GetAll () |> JSON)
        path resourcePath >=> GET >=> getAll