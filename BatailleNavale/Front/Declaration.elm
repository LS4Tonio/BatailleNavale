module Declaration exposing (..)

import Http exposing (..)

type Event = ShowTextBidon | NewText (Result Http.Error User)

-- Coordinate 2 axes
type alias Coordinate = {
    x : Int
    , y : Int
}

-- States for cells and shoots
type States = Missed | Touched | Skined | None

-- Game's state
type GameState = Preparation | InGame | Finished

-- Definition of a boat
type alias Boat = { 
    id : Int
    , name : String
    , length : Int
    , isVertical : Bool
    , isSink : Bool
    , numberHit : Int
}

-- A cell of a grid
type alias Cell = {
    coordinate : Coordinate
    , state : States
}

-- a grid's line
type alias Line = {
    cells : List
}

-- THE grid
type alias Grid = {
    lengthX : Int
    , lengthY : Int
    , lines : List
}

-- A user's definition
type alias User = {
    id : Int
    , name : String
}

-- A player's definition
type alias Player = {
    user : User
    , grid : Grid
}

-- the game
type alias Game = {
    player1 : Player
    , player2 : Player
    , id : Int
    , state : GameState
    , current : Int
}