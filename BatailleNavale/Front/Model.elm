module Model exposing (..)

import Http exposing (..)
import Declaration exposing (..)

type alias Model = {
    user : User
    , grid : Grid
}

-- type alias Model = {
--     game : Game
--     , boats : List
--     , user : User
-- }





