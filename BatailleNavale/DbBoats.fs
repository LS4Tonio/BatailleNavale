namespace BatailleNavale.db.boats

type Coordinate = {
    X: int
    Y: int
}

type SimpleBoat = {
     Name: string
     TopLeftCoordinate: Coordinate
     IsVertical: bool
}

type MayHaveBoat =
    | Boat of SimpleBoat
    | Empty

type GridElement = {
     Boat: MayHaveBoat
     Hit : bool
}

type BoatResponseError =
    | OutOfGrid
    | AllreadyTaken
    | BoatTypeAlreadyPlaced
    | UnknownBoat
    | BoatHasNotAlreadyBeenPlaced

module DbBoats =
    open System.Collections.Generic
    
    let gridSize =
     (100,  100)

    let private Boats = new List<SimpleBoat>()
    let mutable BoatGrid = 
            let ( sizex, sizey) = gridSize
            List.init sizex (fun index -> List.init sizey ( fun index2 -> {GridElement.Boat = MayHaveBoat.Empty; GridElement.Hit = false}))

    
    //let private BoatGrid = new List<List<GridElement>>()
//    let dbInit =    
//        let ( sizex, sizey) = gridSize
//        BoatGrid <- List.init sizex (fun index -> List.init sizey ( fun index2 -> something index index2))
    let private defaultBoats = [
        "carrier"
        "battleship"
        "cruiser"
        "submarine"
        "destroyer"
    ]
    let private boatTypes = [
        ("carrier", 5)
        ("battleship",4)
        ("cruiser",3)
        ("submarine",3)
        ("destroyer",2)
    ]

    let getdefaultboatlistinformation a =
        seq boatTypes



    // Get last coordinates of the boat
    let getEndCoordinates coor boatSize isVertical =
        match isVertical with
        | true ->
            let nco =  coor.Y + boatSize
            {
                X = coor.X
                Y = nco
            }
        | false ->
            let nco = coor.X + boatSize
            {
                X = nco
                Y = coor.Y
            }

    // Check if given y, y is already taken by a boat
    let coordinateInGridHasBoat (x, y) =
        let innerList =  BoatGrid.[x]
        let element = innerList.[y]
        match element.Boat with
            | MayHaveBoat.Boat aBoat -> true
            | Empty -> false

    // Check if x,y is outside the grid
    let coordinatesHasOverlap coor boatSize isVertical =
        match isVertical with
        | true ->
            [coor.Y .. coor.Y+ boatSize]
            |> List.map(fun a -> coordinateInGridHasBoat(coor.X, a))
            |> List.exists( fun a -> a)
        | false ->
            [coor.X .. coor.X+ boatSize]
            |> List.map(fun a -> coordinateInGridHasBoat(a, coor.Y))
            |> List.exists( fun a -> a)

    // Check boat type
    let checkBoatType boatResponse =
        match boatResponse with
            |  Errors.OptionLike.Some aSimpleBoat  ->
                    let result = List.contains(aSimpleBoat.Name) defaultBoats
                    match result with
                        | false -> Errors.Error Errors.UnknownBoat
                        | true -> Errors.OptionLike.Some aSimpleBoat
            | _ -> boatResponse

    // Check coordinates
    let checkCoordinatesAreInsideGrid boatResponse =
        let (sizex, sizey) = gridSize
        match boatResponse with
        | Errors.OptionLike.Some aSimpleBoat ->
            match aSimpleBoat.TopLeftCoordinate.X with
            | b when b >= sizex -> Errors.Error Errors.BoatOutOfGrid
            | _ ->
                match aSimpleBoat.TopLeftCoordinate.Y with
                | c when c >= sizey -> Errors.Error Errors.BoatOutOfGrid
                | _ ->
                    let size = List.tryFind(fun a ->
                        match a with
                        | (b,c) when b = aSimpleBoat.Name -> true
                        | _ -> false ) boatTypes
                    match size with
                    | None -> Errors.Error Errors.UnknownBoat
                    | Option.Some value ->
                        let (n:string, s:int) = value
                        let endCoor = getEndCoordinates aSimpleBoat.TopLeftCoordinate s aSimpleBoat.IsVertical
                        match endCoor.X with
                        | c when c >= sizex -> Errors.Error Errors.BoatOutOfGrid
                        | _ ->
                            match endCoor.Y with
                            | c when c >= sizey -> Errors.Error Errors.BoatOutOfGrid
                            | _ -> boatResponse
        | _ -> boatResponse

    // Check if boat already placed
    let checkBoatAllreadyExists boatResponse =
        match boatResponse with
        |  Errors.OptionLike.Some aSimpleBoat ->
            let result = Boats.Exists(fun a ->
                match a.Name with
                | b when b = aSimpleBoat.Name -> true
                | _ -> false)
            match result with
            | true -> Errors.Error Errors.BoatHasNotAlreadyBeenPlaced
            | false ->  Errors.OptionLike.Some aSimpleBoat
        | _ -> boatResponse

    // Check if boat is in grid
    let checkGridContent boatResponse =
        match boatResponse with
        |  Errors.OptionLike.Some aSimpleBoat ->
            let size = List.tryFind(fun a ->
                match a with
                | (b,c) when  b = aSimpleBoat.Name -> true
                | _ -> false) boatTypes
            match size with
            | None -> Errors.Error Errors.UnknownBoat
            | Option.Some value ->
                let n, s = value
                match coordinatesHasOverlap aSimpleBoat.TopLeftCoordinate s aSimpleBoat.IsVertical with
                    | true -> Errors.Error Errors.LocationAllreadyTaken
                    | false -> boatResponse
        | _ -> boatResponse

    // Get all boats
    let getBoats () =
        Boats |> Seq.map (fun b -> b)

    // Get boat
    let getBoat id =
        Errors.OptionLike.Error    Errors.Errors.NotImplemented

    // Place a bont on grid
    let placeBoatInGrid boat =
        let (name, size) =
            List.find(fun a ->
                match a with
                | (b,c) when  b = boat.Name -> true
                | _ -> false ) boatTypes
        let ab = MayHaveBoat.Boat boat
        match boat.IsVertical with
            | true ->
                [boat.TopLeftCoordinate.Y .. boat.TopLeftCoordinate.Y+ size]
                |> List.map(fun a ->
//                    BoatGrid.[boat.TopLeftCoordinate.X].[a] <- {
//                        GridElement.Boat =  ab
//                        GridElement.Hit = false
//                    }
                             let newGrid = List.mapi(fun i d -> 
                                    match i with 
                                    | index1 when index1 = boat.TopLeftCoordinate.X -> d |> List.mapi(fun i2 b -> 
                                        match i2 with  
                                                | index2 when index2 = a -> {
                                                                                GridElement.Boat =  ab
                                                                                GridElement.Hit = false
                                                                            }
                                                | _ -> b)
                                    | _ -> d ) BoatGrid
                             BoatGrid <- newGrid
                             boat)
                | false ->
                    [boat.TopLeftCoordinate.X .. boat.TopLeftCoordinate.X+ size]
                    |> List.map( fun a ->
//                        BoatGrid.[a].[boat.TopLeftCoordinate.Y] <- {
//                            GridElement.Boat =  ab
//                            GridElement.Hit = false
//                        }
                             let newGrid = List.mapi(fun i d -> 
                                    match i with 
                                    | index1 when index1 = a -> d |> List.mapi(fun i2 b -> 
                                        match i2 with  
                                                | index2 when index2 = boat.TopLeftCoordinate.Y -> {
                                                                                                        GridElement.Boat =  ab
                                                                                                        GridElement.Hit = false
                                                                                                    }
                                                | _ -> b)
                                    | _ -> d ) BoatGrid
                             BoatGrid <- newGrid
                             boat)

    // Add boat on grid
    let placeBoat boat =
        let boat2 = boat |> Errors.OptionLike.Some
        let response:Errors.OptionLike<SimpleBoat> =
            boat2
            |> checkBoatType
            |> checkCoordinatesAreInsideGrid
            |> checkBoatAllreadyExists
            |> checkGridContent
        match response with
        | Errors.OptionLike.Some aSimpleBoat ->
            Boats.Add(aSimpleBoat)
            placeBoatInGrid aSimpleBoat |> ignore
            response
        | _ -> response
    let checkBoatAllreadyExistsforPut boatResponse = 
        match boatResponse with
            | Errors.OptionLike.Some aSimpleBoat  ->
                    let result = Boats.Exists(  fun a -> match a.Name with
                                                            | b when  b = aSimpleBoat.Name -> true
                                                            | _ -> false ) 
                    match result with
                        | true ->  Errors.OptionLike.Some aSimpleBoat
                        | false -> Errors.OptionLike.Error Errors.BoatHasNotAlreadyBeenPlaced
            | _ -> boatResponse
    let checkGridContentForPut boatResponse =
        match boatResponse with
            | Errors.OptionLike.Some aSimpleBoat  ->
                                    let size = List.tryFind( fun a -> match a with
                                                            | (b,c) when  b = aSimpleBoat.Name -> true
                                                            | _ -> false ) boatTypes
                                    match size with
                                        | None -> Errors.OptionLike.Error Errors.UnknownBoat
                                        | Option.Some value ->
                                                    let (n:string, s:int) = value
                                                    match coordinatesHasOverlap aSimpleBoat.TopLeftCoordinate s aSimpleBoat.IsVertical with
                                                        | true -> Errors.OptionLike.Error Errors.LocationAllreadyTaken
                                                        | false -> boatResponse
            | _ -> boatResponse
    let removeBoatFromGrid (oldBoat:SimpleBoat) = //is this right????
        let (name:string, size:int) = List.find( fun a -> match a with
                                                            | (b,c) when  b = oldBoat.Name -> true
                                                            | _ -> false ) boatTypes
        let ab:MayHaveBoat = MayHaveBoat.Empty
        match oldBoat.IsVertical with
                            | true -> [oldBoat.TopLeftCoordinate.Y .. oldBoat.TopLeftCoordinate.Y+ size] |> List.map( fun a -> 
                                                                                        let newGrid = List.mapi(fun i d ->  match i with 
                                                                                                                                | index1 when index1 = oldBoat.TopLeftCoordinate.X -> d |> List.mapi(fun i2 b ->  match i2 with  
                                                                                                                                                                                                                    | index2 when index2 = a -> {
                                                                                                                                                                                                                                                    GridElement.Boat =  ab
                                                                                                                                                                                                                                                    GridElement.Hit = false
                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                    | _ -> b)
                                                                                                                                | _ -> d ) BoatGrid
                                                                                        BoatGrid <- newGrid
                                                                                        oldBoat)
//                                                                                                                               BoatGrid.[oldBoat.TopLeftCoordinate.X].[a] <- {
//                                                                                                                                                        GridElement.Boat =  ab
//                                                                                                                                                        GridElement.Hit = false
//                                                                                                                                                        }
                                                                                                                        
                            
                                   
                                                                                                            
                            | false -> [oldBoat.TopLeftCoordinate.X .. oldBoat.TopLeftCoordinate.X+ size] |> List.map( fun a -> 
                                                                                        let newGrid = List.mapi(fun i d ->  match i with 
                                                                                                                                | index1 when index1 = a -> d |> List.mapi(fun i2 b ->  match i2 with  
                                                                                                                                                                                                    | index2 when index2 = oldBoat.TopLeftCoordinate.Y -> {
                                                                                                                                                                                                                                                                GridElement.Boat =  ab
                                                                                                                                                                                                                                                                GridElement.Hit = false
                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                    | _ -> b)
                                                                                                                                | _ -> d ) BoatGrid
                                                                                        BoatGrid <- newGrid
                                                                                        oldBoat)
//                                                                                                                          fun a -> BoatGrid.[a].[oldBoat.TopLeftCoordinate.Y] <- {
//                                                                                                                                                        GridElement.Boat =  ab
//                                                                                                                                                        GridElement.Hit = false
//                                                                                                                                                        }
                                                                                                                               // oldBoat)
                                    

        //todo : checks again! (different overlap check, because overlapping with itself wont be a pbm), check we already have it in list!
    let updateBoat aSimpleBoat =
        let boat2 = aSimpleBoat |> Errors.OptionLike.Some
        let response =  boat2 |> checkBoatType |> checkCoordinatesAreInsideGrid |> checkBoatAllreadyExistsforPut |> checkGridContentForPut
        match response with
            | Errors.OptionLike.Some aSimpleBoat ->
                                    let oldBoat:SimpleBoat=  Boats.Find(fun a -> match a.Name with
                                                                                    | c when c = aSimpleBoat.Name -> true
                                                                                    | _ -> false)
                                    oldBoat |> removeBoatFromGrid |> ignore
                                    Boats.RemoveAll(fun a -> match a.Name with
                                                                    | c when c = aSimpleBoat.Name -> true
                                                                    | _ -> false) |> ignore
                                    Boats.Add(aSimpleBoat) |> ignore
                                    placeBoatInGrid aSimpleBoat  |> ignore
                                    response
            | _ -> response     



    let deleteBoat aSimpleBoat =
        let boat2 = aSimpleBoat |> Errors.OptionLike.Some
        let response =  boat2 |> checkBoatType |> checkBoatAllreadyExistsforPut // todo : vérifier qu'on le trouve bien là où on nous le dit?
        match response with
            | Errors.OptionLike.Some aSimpleBoat ->
                                    let oldBoat:SimpleBoat=  Boats.Find(fun a -> match a.Name with
                                                                                    | c when c = aSimpleBoat.Name -> true
                                                                                    | _ -> false)
                                    oldBoat |> removeBoatFromGrid  |> ignore
                                    Boats.RemoveAll(fun a -> match a.Name with
                                                                        | c when c = aSimpleBoat.Name -> true
                                                                        | _ -> false)  |> ignore
                                    response
            | _ -> response     


