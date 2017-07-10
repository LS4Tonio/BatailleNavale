module Update exposing (update)

import Html exposing (Html, Attribute, div, input, text, table, tr, td, thead, th)
import Html.Attributes as A
import Html.Events exposing (onInput)
import Draggable
import Model exposing (Model)
import Msg exposing (Msg)

update : Msg -> Model -> Model
update msg model =
  case msg of
    Msg.Increment ->
      model + 1

    Msg.Decrement ->
      model - 1

    Msg.Reset ->
      0