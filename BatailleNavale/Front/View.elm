module View exposing (..)

import Html exposing (..)
import Html.Attributes as A
import Html.Events exposing (onInput, onClick)
import Draggable
import Declaration exposing (..)
import Update exposing (..)
import Model exposing (..)
import Material.Grid exposing (grid, cell, size, Device(..))


view : Model -> Html Event
view model =
  div []
    [ div [] [ text model.user.name ]
    , button [ onClick ShowTextBidon ] [ text "Please work !!" ]
    , table [ A.style [("border-collapse", "collapse")]]
        [
        tbody [] [
        tr []
            [ th [ ] [ ]
                , th [ ] [ text "A" ]
                , th [ ] [ text "B" ]
                , th [ ] [ text "C" ]
                , th [ ] [ text "D" ]
                , th [ ] [ text "E" ]
                , th [ ] [ text "F" ]
                , th [ ] [ text "G" ]
                , th [ ] [ text "H" ]
                , th [ ] [ text "I" ]
                , th [ ] [ text "J" ]
            ]
            , tr []
            [ th [ ] [ text "1" ]
                , th [ ] [ ]
                , th [ ] [ ]
                , th [ ] [ ]
                , th [ ] [ ]
                , th [ ] [ ]
                , th [ ] [ ]
                , th [ ] [ ]
                , th [ ] [ ]
                , th [ ] [ ]
                , th [ ] [ ]
            ]
        ]
        ]
    ]


--toTableLine : List Cell -> Html msg
--toTableLine ligne =
--    List.concat [
--        tr []
--        [ th [ ] [ text "1" ]
--        , List.map toTableRow ligne
--        ]
--    ]
--
--toTableRow: Cell -> Html msg
--toTableRow cell =
--    td [ A.style [("border", "1px solid black")]  ] [ ]