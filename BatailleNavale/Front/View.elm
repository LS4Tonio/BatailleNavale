module View exposing (view)

import Html exposing (Html, Attribute, div, input, text, button)
import Html.Attributes as A
import Html.Events exposing (onInput, onClick)
import Draggable
import Model exposing (Model)
import Msg exposing (Msg)

view : Model -> Html Msg
view model =
  div []
    [ button [ onClick Msg.Decrement ] [ text "-" ]
    , div [] [ text (toString model) ]
    , button [ onClick Msg.Increment ] [ text "+" ]
    , button [ onClick Msg.Reset ] [ text "0" ]
    ]