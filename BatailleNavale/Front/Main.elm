module Main exposing (..)

import Update
import View
import Model
import Html exposing (Html, Attribute, div, input, text, table, tr, td, thead, th)
import Html.Attributes as A
import Html.Events exposing (onInput)
import Draggable

main =
  Html.beginnerProgram { model = Model.model, view = View.view, update = Update.update }