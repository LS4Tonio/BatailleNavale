import Html exposing (Html, Attribute, div, input, text, button, h4, p)
import Html.Attributes as A
import Html.Events exposing (onInput, onClick)
import Draggable
import Msg exposing (..)
import Update exposing (..)
import Material.Grid exposing (grid, cell, size, Device(..))

-- MODEL

type alias Model =
  { topic : String
  , gifUrl : String
  }

init : (Model, Cmd Msg)
init =
  (Model "cats" "waiting.gif", Cmd.none)


-- UPDATE

type Msg = MorePlease

update : Msg -> Model -> (Model, Cmd Msg)
update msg model =
  case msg of
    MorePlease ->
      (model, Cmd.none)


-- VIEW

view : Model -> Html Msg
view model =
  div []
    [ h2 [] [text model.topic]
    , img [src model.gifUrl] []
    , button [ onClick MorePlease ] [ text "More Please!" ]
    ]