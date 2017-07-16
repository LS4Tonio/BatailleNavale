//namespace BatailleNavale.Rest
//
//[<AutoOpen>]
//module Rest2 =
//    open Newtonsoft.Json
//    open Newtonsoft.Json.Serialization
//    open Suave
//    open Suave.Operators
//    open Suave.Successful
//    open Suave.Filters
//    open Suave.RequestErrors
//    open BatailleNavale.db.boats
//    //TENTATIVE de voir si on arrive a chopper les param du webpart ET du JSON. Evidement à remanier pour ce que l'on souhaite
//    
//    let getResourceFromReq (req: HttpRequest)  :SimpleBoat  =
//        let getString rawForm =
//            System.Text.Encoding.UTF8.GetString(rawForm)
//        req.rawForm |> getString |> fromJson<SimpleBoat>
//
//    let anything (a:SimpleBoat) (b:int,c:int) :SimpleBoat = a
////    //game, //player, // and all the placeboat stuff hmmm so placeboat/1(game)/1(player)/whateverelse
//    let testapp : WebPart = 
//                            choose
//                                    [ 
//                                        GET >=> pathScan "/add/%d/%d" (fun (a,b) -> OK((a + b).ToString()))
////                                        POST >=> 
////                                            let partialpathscan  = pathScan  "/add/%d/%d"
////                                            let partialreq = getResourceFromReq >> (fun a  -> a)
////                                            partialpathscan (fun(a,b)->  partialreq a b  ) >> JSON
//                                        POST >=> pathScan "/add/%d/%d" (fun (a,b) -> OK((a + b).ToString())) >=> request (getResourceFromReq >> anything >> JSON)
////                                            let partialreq = getResourceFromReq request  //  >> (fun a  -> a)
////                                            pathScan  "/add/%d/%d" ((fun(a,b)-> anything partialreq a b)  >> JSON)
//                                        NOT_FOUND "Found no handlers" 
//                                    ]
//                                    //
////
////                                    
////        choose [
////            path resourcePath >=> choose [
////                GET >=> getAll
////                POST >=>
////                    request (getResourceFromReq >> resource.Create >> JSON)
////                PUT >=>
////                    request (getResourceFromReq >> resource.Update >> handleResource2 badRequest2)
////            ]
////            DELETE >=> pathScan resourceIdPath deleteResourceById
////            GET >=> pathScan playerIdPath getResourceById
////            PUT >=> pathScan resourceIdPath updateResourceById
////            HEAD >=> pathScan resourceIdPath isResourceExists
////        ]