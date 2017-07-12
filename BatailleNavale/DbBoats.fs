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

    let private Boats = new List<SimpleBoat>()
    let private BoatGrid = List<List<GridElement>>()
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
            | Boat aSimpleBoat  ->
                    let result = List.contains(aSimpleBoat.Name) defaultBoats
                    match result with
                        | false -> Errors.Error Errors.UnknownBoat
                        | true -> BoatResponse.Boat aSimpleBoat
            | _ -> boatResponse

    // Check coordinates
    let checkCoordinatesAreInsideGrid boatResponse =
        match boatResponse with
        | Boat aSimpleBoat ->
            match aSimpleBoat.TopLeftCoordinate.X with
            | b when b > 99 -> Errors.Error Errors.BoatOutOfGrid
            | _ ->
                match aSimpleBoat.TopLeftCoordinate.Y with
                | c when c > 99 -> Errors.Error Errors.BoatOutOfGrid
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
                        | c when c > 99 -> Errors.Error Errors.BoatOutOfGrid
                        | _ ->
                            match endCoor.Y with
                            | c when c > 99 -> Errors.Error Errors.BoatOutOfGrid
                            | _ -> boatResponse
            | _ -> boatResponse
        | Empty -> boatResponse

    // Check if boat already placed
    let checkBoatAllreadyExists boatResponse =
        match boatResponse with
        | Boat aSimpleBoat ->
            let result = Boats.Exists(fun a ->
                match a.Name with
                | b when b = aSimpleBoat.Name -> true
                | _ -> false)
            match result with
            | true -> Errors.Error Errors.BoatHasNotAlreadyBeenPlaced
            | false -> BoatResponse.Boat aSimpleBoat
        | _ -> boatResponse

    // Check if boat is in grid
    let checkGridContent boatResponse =
        match boatResponse with
        | Boat aSimpleBoat ->
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
        Errors.Errors.NotImplemented

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
                    BoatGrid.[boat.TopLeftCoordinate.X].[a] <- {
                        GridElement.Boat =  ab
                        GridElement.Hit = false
                    }
                    boat)
                | false ->
                    [boat.TopLeftCoordinate.X .. boat.TopLeftCoordinate.X+ size]
                    |> List.map( fun a ->
                        BoatGrid.[a].[boat.TopLeftCoordinate.Y] <- {
                            GridElement.Boat =  ab
                            GridElement.Hit = false
                        }
                        boat)

    // Add boat on grid
    let placeBoat boat =
        let response =
            boat
            |> checkBoatType
            |> checkCoordinatesAreInsideGrid
            |> checkBoatAllreadyExists
            |> checkGridContent
        match response with
        | Boat aSimpleBoat ->
            Boats.Add(aSimpleBoat)
            let l = placeBoatInGrid aSimpleBoat
            response
        | _ -> response