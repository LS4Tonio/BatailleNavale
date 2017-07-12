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
        Create : 'a -> 'a
        Update : 'a -> 'a option
        Delete : int -> unit
        GetById : int -> 'a option
        UpdateById : int -> 'a -> 'a option
        IsExists : int -> bool
    }
    type RestResource2<'a> = {
        GetAll : unit -> 'a seq
        Create : 'a -> 'a Errors.OptionLike
        Update : 'a -> 'a Errors.OptionLike
        Delete : int -> unit
        GetById : int -> 'a option
        UpdateById : int -> 'a -> 'a Errors.OptionLike
        IsExists : int -> bool
    }

//    let JSON2 v:Errors.OptionLike<'a> =
//        let serializer = new JsonSerializerSettings()
//        serializer.ContractResolver <- new CamelCasePropertyNamesContractResolver()        
//        JsonConvert.SerializeObject((Errors.OptionLikeToOption v), serializer)
//        |> OK
//        >=> Writers.setMimeType "application/json; charset=utf-8"

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
//    let handleResource requestError = function
//            | Some r -> r |> JSON
//            | _ -> requestError
    // string -> RestResource<'a> -> WebPart
    let rest resourceName resource =
        let resourcePath = "/" + resourceName
        let resourceIdPath =
            new PrintfFormat<(int -> string), unit, string, string, int>(resourcePath + "/%d")

        let badRequest3 error =
                                match error with
                                | Errors.Errors.BoatHasNotAlreadyBeenPlaced -> "Boat has not been placed"
                                | _  -> "Resource not found"
        
        let badRequest2 error = BAD_REQUEST (error.ToString())  //(badRequest3 error)         //"Resource not found"//put-être que je fais de la merde, j'en sais rien...~~
        let badRequest = BAD_REQUEST "Resource not found"

        let getAll = warbler (fun _ -> resource.GetAll () |> JSON)
        let handleResource2 requestError value = match value with
                                                                                | Errors.OptionLike.Some r -> r |>  JSON
                                                                                | Errors.OptionLike.Error r -> r |> requestError//
        let handleResource requestError value = match value with
                                                        | Some r -> r |> JSON
                                                        | _ -> requestError

        let getResourceById =
            resource.GetById >> handleResource (NOT_FOUND "Resource not found")
        let updateResourceById id = // x =
//            request  handleResource2( badRequest  (resource.UpdateById (id (getResourceFromReq  x))  ))
           // request |> getResourceFromReq x |> resource.UpdateById id |> handleResource2 badRequest
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


//module CORS =
//  [<RequireQualifiedAccess>]
//  type InclusiveOption<'T> =
//    | None
//    | Some of 'T
//    | All
//  type CORSConfig =
//    {
//      allowedUris             : InclusiveOption<string list>
//      allowedMethods          : InclusiveOption<HttpMethod list>
//      allowCookies            : bool
//      exposeHeaders           : InclusiveOption<string list>
//      maxAge                  : int option }
//    static member allowedUris_           : Property<CORSConfig, InclusiveOption<string list>>
//    static member allowedMethods_        : Property<CORSConfig, InclusiveOption<HttpMethod list>>
//    static member allowCookies_          : Property<CORSConfig, bool>
//    static member exposeHeaders_         : Property<CORSConfig, InclusiveOption<string list>>
//    static member maxAge_                : Property<CORSConfig, int option>
//  val cors : CORSConfig:(CORSConfig) -> WebPart
//  val defaultCORSConfig : CORSConfig
