module Main exposing (..)

import Update
import View
import Model exposing (Model)
import Declaration exposing (..)
import Html exposing (Html, Attribute, div, input, text, table, tr, td, thead, th)
import Html.Attributes as A
import Html.Events exposing (onInput)
import Draggable

main =
    Html.program 
    { init = Update.init (User 1 "wait")
    , view = View.view
    , update = Update.update
    , subscriptions = subscriptions
    }

subscriptions : Model -> Sub Event
subscriptions model =
    Sub.none