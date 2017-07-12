module Errors

type Errors = 
    | BoatOutOfGrid
    | LocationAllreadyTaken
    | BoatTypeAlreadyPlaced
    | UnknownBoat
    | BoatHasNotAlreadyBeenPlaced //for update
    | UnknownUser
    | UnknownGame

type OptionLike<'a> =       // use a generic definition
   | Some of 'a           // valid value
   | Error of Errors                 // missing


let OptionLikeToOption  anOptionLike =
    match anOptionLike with 
        | Some a -> Option.Some a
        | Error b -> Option.Some (b.ToString())