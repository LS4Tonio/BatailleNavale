module Update exposing (..)

import Html exposing (Html, Attribute, div, input, text, table, tr, td, thead, th)
import Html.Attributes as A
import Html.Events exposing (onInput)
import Http exposing (..)
import Draggable
import Model exposing (Model)
import Declaration exposing (..)
import Json.Decode as Decode

init : User -> (Model, Cmd Event)
init user =
  ( Model user
  , getUsers user.id
  )

update : Event -> Model -> ( Model, Cmd Event )
update event model =
  case event of
    ShowTextBidon ->
      (model, getUsers model.user.id)

    NewText (Ok newUser) ->
      (Model newUser, Cmd.none)

    NewText (Err _) ->
      (model, Cmd.none)

getUsers : Int -> Cmd Event
getUsers id =
    let
      url = "http://localhost:8080/users/1"
    in
      Http.send NewText (Http.get url decodeUser)

decodeUser : Decode.Decoder User
decodeUser =
    Decode.map2 User
        (Decode.field "id" Decode.int)
        (Decode.field "username" Decode.string)