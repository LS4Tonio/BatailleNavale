module Rules

open System.Collections.Generic

let  defaultBoats = ["carrier"; "battleship"; "cruiser"; "submarine"; "destroyer"]
let boatTypes = [("carrier", 5); ("battleship",4); ("cruiser",3); ("submarine",3); ("destroyer",2)]

type SimpleBoat = {
     Name: string
     TopLeftCoordinate: BatailleNavale.db.boats.Coordinate
     IsVertical: bool
}


type MayHaveBoat = 
    | Boat of SimpleBoat
    | Empty

type GridElement = {
     Boat: MayHaveBoat
     Hit : bool
}
type BoatResponseError = //moved to errors
    | OutOfGrid
    | AllreadyTaken
    | BoatTypeAlreadyPlaced
    | UnknownBoat
    | BoatHasNotAlreadyBeenPlaced //for update

//type OptionLike<'a> =       // use a generic definition
//   | Some of 'a           // valid value
//   | Error of BoatResponseError                 // missing

//type BoatResponse = 
//    | Boat of SimpleBoat
//    | Error of BoatResponseError

let private Boats = new List<SimpleBoat>()
let private BoatGrid = List<List<GridElement>>()

let checkBoatType boatResponse = 
    match boatResponse with
        | Errors.OptionLike.Some aSimpleBoat  ->
                let result = List.contains(aSimpleBoat.Name ) defaultBoats
                match result with
                    | false -> Errors.OptionLike.Error Errors.UnknownBoat
                    | true -> Errors.OptionLike.Some aSimpleBoat
        | _ -> boatResponse

let checkBoatAllreadyExists boatResponse = 
    match boatResponse with
        | Errors.OptionLike.Some aSimpleBoat  ->
                let result = Boats.Exists(  fun a -> match a.Name with
                                                        | b when  b = aSimpleBoat.Name -> true
                                                        | _ -> false ) 
                match result with
                    | true -> Errors.OptionLike.Error Errors.BoatTypeAlreadyPlaced
                    | false -> Errors.OptionLike.Some aSimpleBoat
        | _ -> boatResponse
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

let getEndCoordinates (coor:BatailleNavale.db.boats.Coordinate) (boatSize:int) (isVertical:bool) = 
                    match isVertical with
                        | true ->  let nco =  coor.Y + boatSize
                                   let truc =  { BatailleNavale.db.boats.Coordinate.X = coor.X;  BatailleNavale.db.boats.Coordinate.Y = nco }
                                   truc
                        | false -> let nco = coor.X + boatSize
                                   let truc =  { BatailleNavale.db.boats.Coordinate.X = nco;  BatailleNavale.db.boats.Coordinate.Y = coor.Y }
                                   truc

let checkCoordinatesAreInsideGrid boatResponse = 
    match boatResponse with
        | Errors.OptionLike.Some aSimpleBoat  ->
                match aSimpleBoat.TopLeftCoordinate.X with
                    | b when b > 99 ->Errors.OptionLike.Error Errors.BoatOutOfGrid
                    | _ -> 
                        match aSimpleBoat.TopLeftCoordinate.Y with
                            | c when c > 99 -> Errors.OptionLike.Error Errors.BoatOutOfGrid
                            | _ -> 
                                let size = List.tryFind( fun a -> match a with
                                                        | (b,c) when  b = aSimpleBoat.Name -> true
                                                        | _ -> false ) boatTypes
                                match size with
                                    | None -> Errors.OptionLike.Error Errors.UnknownBoat
                                    | Option.Some value ->
                                                let (n:string, s:int) = value
                                                let endCoor = getEndCoordinates aSimpleBoat.TopLeftCoordinate s aSimpleBoat.IsVertical
                                                match endCoor.X with
                                                    | c when c > 99 -> Errors.OptionLike.Error Errors.BoatOutOfGrid
                                                    | _ -> 
                                                        match endCoor.Y with
                                                            | c when c > 99 -> Errors.OptionLike.Error Errors.BoatOutOfGrid
                                                            | _ -> boatResponse
        | _ -> boatResponse

let coordinateInGridHasBoat (x,y) = 
        let innerList =  BoatGrid.[x]
        let element = innerList.[y]
        match element.Boat with
            | MayHaveBoat.Boat aBoat -> true
            | Empty -> false
let coordinateInGridHasBoatForPut (x,y) (boat:SimpleBoat) = 
        let innerList =  BoatGrid.[x]
        let element = innerList.[y]
        match element.Boat with
            | MayHaveBoat.Boat aBoat -> 
                            match aBoat.Name with
                                | c when c = boat.Name -> false
                                | _ -> true
            | Empty -> false
   
let coordinatesHasOverlapForPut (coor:BatailleNavale.db.boats.Coordinate) (boatSize:int) (isVertical:bool) (boat:SimpleBoat) =
                    match isVertical with
                        | true -> [coor.Y .. coor.Y+ boatSize] |> List.map( fun a ->coordinateInGridHasBoatForPut(coor.X, a) boat ) |> List.exists( fun a -> a) 
                        | false -> [coor.X .. coor.X+ boatSize] |> List.map( fun a ->coordinateInGridHasBoatForPut(a, coor.Y) boat) |> List.exists( fun a -> a) 
let coordinatesHasOverlap (coor:BatailleNavale.db.boats.Coordinate) (boatSize:int) (isVertical:bool) =
                    match isVertical with
                        | true -> [coor.Y .. coor.Y+ boatSize] |> List.map( fun a ->coordinateInGridHasBoat(coor.X, a) ) |> List.exists( fun a -> a) 
                        | false -> [coor.X .. coor.X+ boatSize] |> List.map( fun a ->coordinateInGridHasBoat(a, coor.Y) ) |> List.exists( fun a -> a) 

let checkGridContent boatResponse =
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

let placeBoatInGrid (boat:SimpleBoat) = //is this right????
    let (name:string, size:int) = List.find( fun a -> match a with
                                                        | (b,c) when  b = boat.Name -> true
                                                        | _ -> false ) boatTypes
    let ab:MayHaveBoat = MayHaveBoat.Boat boat
    match boat.IsVertical with
                        | true -> [boat.TopLeftCoordinate.Y .. boat.TopLeftCoordinate.Y+ size] |> List.map( fun a -> BoatGrid.[boat.TopLeftCoordinate.X].[a] <- {
                                                                                                                                                    GridElement.Boat =  ab
                                                                                                                                                    GridElement.Hit = false
                                                                                                                                                    }
                                                                                                                     boat
                                                                                                            ) 
                        | false -> [boat.TopLeftCoordinate.X .. boat.TopLeftCoordinate.X+ size] |> List.map( fun a -> BoatGrid.[a].[boat.TopLeftCoordinate.Y] <- {
                                                                                                                                                    GridElement.Boat =  ab
                                                                                                                                                    GridElement.Hit = false
                                                                                                                                                    }
                                                                                                                      boat
                                                                                                            )
let removeBoatFromGrid (oldBoat:SimpleBoat) = //is this right????
    let (name:string, size:int) = List.find( fun a -> match a with
                                                        | (b,c) when  b = oldBoat.Name -> true
                                                        | _ -> false ) boatTypes
    let ab:MayHaveBoat = MayHaveBoat.Empty
    match oldBoat.IsVertical with
                        | true -> [oldBoat.TopLeftCoordinate.Y .. oldBoat.TopLeftCoordinate.Y+ size] |> List.map( fun a -> BoatGrid.[oldBoat.TopLeftCoordinate.X].[a] <- {
                                                                                                                                                    GridElement.Boat =  ab
                                                                                                                                                    GridElement.Hit = false
                                                                                                                                                    }
                                                                                                                           oldBoat)
                                   
                                                                                                            
                        | false -> [oldBoat.TopLeftCoordinate.X .. oldBoat.TopLeftCoordinate.X+ size] |> List.map( fun a -> BoatGrid.[a].[oldBoat.TopLeftCoordinate.Y] <- {
                                                                                                                                                    GridElement.Boat =  ab
                                                                                                                                                    GridElement.Hit = false
                                                                                                                                                    }
                                                                                                                            oldBoat)
                                    

    //todo : checks again! (different overlap check, because overlapping with itself wont be a pbm), check we already have it in list!
let updateBoat aSimpleBoat =
    let response =  aSimpleBoat |> checkBoatType |> checkCoordinatesAreInsideGrid |> checkBoatAllreadyExistsforPut |> checkGridContentForPut
    match response with
        | Errors.OptionLike.Some aSimpleBoat ->
                                let oldBoat:SimpleBoat=  Boats.Find(fun a -> match a.Name with
                                                                                | c when c = aSimpleBoat.Name -> true
                                                                                | _ -> false)
                                let unused = oldBoat |> removeBoatFromGrid
                                let unused = Boats.RemoveAll(fun a -> match a.Name with
                                                                                | c when c = aSimpleBoat.Name -> true
                                                                                | _ -> false)
                                Boats.Add(aSimpleBoat)
                                let unused = placeBoatInGrid aSimpleBoat
                                response
        | _ -> response             
let deleteBoat aSimpleBoat =
    let response =  aSimpleBoat |> checkBoatType |> checkBoatAllreadyExistsforPut // todo : vérifier qu'on le trouve bien là où on nous le dit?
    match response with
        | Errors.OptionLike.Some aSimpleBoat ->
                                let oldBoat:SimpleBoat=  Boats.Find(fun a -> match a.Name with
                                                                                | c when c = aSimpleBoat.Name -> true
                                                                                | _ -> false)
                                let unused = oldBoat |> removeBoatFromGrid
                                let unused = Boats.RemoveAll(fun a -> match a.Name with
                                                                                | c when c = aSimpleBoat.Name -> true
                                                                                | _ -> false)
                                response
        | _ -> response                                 

// PlaceBoat
//check coordinates are in grid.
//check not where another boat is
//check boat type/number!
//add boat to boat list, add boat to boatgrid
let placeBoat aSimpleBoat =
    let response:Errors.OptionLike<SimpleBoat> =  aSimpleBoat |> checkBoatType |> checkCoordinatesAreInsideGrid |> checkBoatAllreadyExists |> checkGridContent
    match response with
        | Errors.OptionLike.Some aSimpleBoat ->
                let unused = Boats.Add(aSimpleBoat)
                let unused = placeBoatInGrid aSimpleBoat
                response
        | _ -> response
    response
    
let getAll () =
    Boats |> Seq.map (fun m -> m)

//==========================================================

//list7 & resultList2 is the proper example


type Untruc = int

type Deuxtruc =
    | UnInt of Untruc
    | Empty

let returndeuxtruc p =
    match p with
    | a when a<10 -> Deuxtruc.UnInt p
    | _ -> Deuxtruc.Empty



let aggregateString s1 s2 = 
    match String.length s1, String.length s2 with
    | l1, l2 when l1>  l2 -> s1
    | _ -> s2

    
let something index2 index3 = 
      index2 * index3  
let printsomething s1 =
    let list = [["a";"absg"];["a";"ag"];["a";"agd"];["a";"agf"]]
    
    (* using init method *)
    let list5 = List.init 5 (fun index -> (index, index * index, index * index * index))
    let list6 = List.init 5 (fun index -> List.init ( something index 1))
    let list7 = List.init 5 (fun index -> List.init 3 ( fun index2 -> something index index2))
    printfn "The list (5): %A" list5
    printfn "The list: %A" list
    let resultList = List.collect (fun smthg -> printfn "The list (7): %A" smthg
                                                ["a"]
                                    )   list7 
    printfn "The list (6): %A" list6
    printfn "The list (7): %A" list7
    printfn "The resultList: %A" resultList

    let itochange1 = 1;
    let itochange2 = 2;
    let itochangeto = 100;
    let resultList4 =  list7 |> List.mapi  (fun i a -> a  ) 
    let resultList3 =  list7 |> List.mapi  (fun i a -> 
        match i with | itochange -> a
                     | _ -> a  ) 
                                                                                                                                
    let resultList2 = list7 |> List.mapi(fun i a -> 
        match i with 
        | index1 when index1 = itochange1 -> a |> List.mapi(fun i2 b -> 
            match i2 with  
                  | index2 when index2 = itochange2 -> itochangeto
                  | _ -> b)
        | _ -> a ) 

    printfn "The resultList2: %A" resultList2
    
    printfn "%s" (["a";"ab";"abc";"abcd"] |> List.reduce aggregateString)

