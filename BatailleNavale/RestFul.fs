namespace BatailleNavale.Rest

[<AutoOpen>]
module RestFul =
    open Newtonsoft.Json
    open Newtonsoft.Json.Serialization
    open Suave
    open Suave.Operators
    open Suave.Successful
    open Suave.Filters
    open Suave.RequestErrors

    type RestResource<'a> = {
        GetAll : unit -> 'a seq
        Create : 'a -> 'a Errors.OptionLike
        Update : 'a -> 'a Errors.OptionLike
        Delete : int -> unit
        GetById : int -> 'a option
        UpdateById : int -> 'a -> 'a Errors.OptionLike
        IsExists : int -> bool
    }

    let JSON v =
        let serializer = new JsonSerializerSettings()
        serializer.ContractResolver <- new CamelCasePropertyNamesContractResolver()
        JsonConvert.SerializeObject( v, serializer)
        |> OK
        >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json =
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a

    let getResourceFromReq<'a> (req: HttpRequest) =
        let getString rawForm =
            System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>

    let rest resourceName resource =
        let resourcePath = "/" + resourceName
        let resourceIdPath =
            new PrintfFormat<(int -> string), unit, string, string, int>(resourcePath + "/%d")

        let badRequest = BAD_REQUEST "Resource not found"
        let badRequest2 error = BAD_REQUEST (error.ToString())
        let badRequest3 error =
            match error with
            | Errors.Errors.BoatHasNotAlreadyBeenPlaced -> "Boat has not been placed"
            | _  -> "Resource not found"

        let handleResource requestError value =
            match value with
            | Some r -> r |> JSON
            | _ -> requestError

        let handleResource2 requestError value =
            match value with
                | Errors.OptionLike.Some r -> r |>  JSON
                | Errors.OptionLike.Error r -> r |> requestError

        let getAll = warbler (fun _ -> resource.GetAll () |> JSON)

        let getResourceById =
            resource.GetById >> handleResource (NOT_FOUND "Resource not found")

        let updateResourceById id =
            request (getResourceFromReq >> (resource.UpdateById id) >> handleResource2 badRequest2)

        let deleteResourceById id =
            resource.Delete id
            NO_CONTENT

        let isResourceExists id =
            match resource.IsExists id with
            | true -> OK ""
            | false -> NOT_FOUND ""

        choose [
            path resourcePath >=> choose [
                GET >=> getAll
                POST >=>
                    request (getResourceFromReq >> resource.Create >> JSON)
                PUT >=>
                    request (getResourceFromReq >> resource.Update >> handleResource2 badRequest2)
            ]
            DELETE >=> pathScan resourceIdPath deleteResourceById
            GET >=> pathScan resourceIdPath getResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath isResourceExists
        ]