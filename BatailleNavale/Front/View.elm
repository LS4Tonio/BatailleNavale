module View exposing (view)

import Html exposing (Html, Attribute, div, input, text, button, h4, p)
import Html.Attributes as A
import Html.Events exposing (onInput, onClick)
import Draggable
import Model exposing (Model)
import Msg exposing (Msg)
import Material.Grid exposing (grid, cell, size, Device(..))

view : Model -> Html Msg
view model =
  div []
    [ button [ onClick Msg.Decrement ] [ text "-" ]
    , div [] [ text (toString model) ]
    , button [ onClick Msg.Increment ] [ text "+" ]
    , button [ onClick Msg.Reset ] [ text "0" ]
    , grid []
    [ cell [ size All 4 ]
        [ h4 [] [text "Cell 1"]
        ]
    , cell [ Material.Grid.offset All 2, size All 4 ]
        [ h4 [] [text "Cell 2"]
        , p [] [text "This cell is offset by 2"]
        ]
    , cell [ size All 6 ]
        [ h4 [] [text "Cell 3"]
        ]
    , cell [ size Tablet 6, size Desktop 12, size Phone 2 ]
        [ h4 [] [text "Cell 4"]
        , p [] [text "Size varies with device"]
        ]
       ]
    ]