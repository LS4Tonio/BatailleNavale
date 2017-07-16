namespace BatailleNavale.db.games

open System.Collections.Generic
open BatailleNavale.db.boats

type GameState =
    | WaitingForPlayer = 0
    | Preparation = 1
    | InGame = 2
    | Finished = 3

type CellState =
    | Empty
    | Missed
    | Touched
    | Sinked

type Coordinates = {
    X: int
    Y: int
}

type Cell = {
    Coordinates: Coordinates
    State: CellState
}

type Grid = {
    LengthX: int
    LengthY: int
    Cells: List<Cell>
}

type Game = {
    Id: int
    Player1: int
    Player2: int
    State: GameState
    Current: int
    Winner: int
}
type PlayerBoats = {
    aPlayerBoats: List<SimpleBoat> //DbBoats.Boats
    aPlayerGrid: GridElement list list //DbBoats.BoatGrid
}

type GameServerOnly = {
    GameId: int
    PlayerOne: PlayerBoats
    PlayerTwo: PlayerBoats
}

module DbGames =
    let private gameStorage = new Dictionary<int, Game>()
    let private gameStorage2 = new Dictionary<int, GameServerOnly>()

    // Get all games waiting for player
    let getAll () =
        gameStorage.Values
            |> Seq.map (fun m -> m)
            |> Seq.filter (fun m -> m.State = GameState.WaitingForPlayer)

    // Get game
    let getById id =
        match gameStorage.ContainsKey(id) with
        | true -> Some gameStorage.[id]
        | false -> None

    // Add boat on grid
    let placeBoat2 (boat:SimpleBoat) (boats: PlayerBoats) (gameid:int) (firstplayer:bool) :Errors.OptionLike<SimpleBoat> =
        let boat2 = boat |> Errors.OptionLike.Some
        let response:Errors.OptionLike<SimpleBoat> =
            boat2
            |> DbBoats.checkBoatType
            |> DbBoats.checkCoordinatesAreInsideGrid
            |> (DbBoats.checkBoatAllreadyExists2 boats.aPlayerBoats)
            |> (DbBoats.checkGridContent2 boats.aPlayerGrid)
        match response with
        | Errors.OptionLike.Some aSimpleBoat ->
            boats.aPlayerBoats.Add(aSimpleBoat)
            let newGrid = DbBoats.placeBoatInGrid2 aSimpleBoat boats.aPlayerGrid
            //todo modify 
            let currentGame2 = gameStorage2.[gameid]
            let playerone =  match firstplayer with
                                | true -> { aPlayerBoats = boats.aPlayerBoats; aPlayerGrid = newGrid }
                                | false -> { aPlayerBoats = currentGame2.PlayerOne.aPlayerBoats; aPlayerGrid =  currentGame2.PlayerOne.aPlayerGrid }
            let playertwo =  match firstplayer with
                                | false -> { aPlayerBoats = boats.aPlayerBoats; aPlayerGrid = newGrid }
                                | true -> { aPlayerBoats =  currentGame2.PlayerTwo.aPlayerBoats; aPlayerGrid =  currentGame2.PlayerTwo.aPlayerGrid }
            gameStorage2.[gameid] <-
             {
                GameServerOnly.GameId = currentGame2.GameId
                GameServerOnly.PlayerOne = playerone
                GameServerOnly.PlayerTwo = playertwo
            }
            response
        | _ -> response

    let  placeBoat3 (a:SimpleBoat) (gameId:int,playerId:int) :Errors.OptionLike<SimpleBoat> = //todo checks...
            let currentGame = gameStorage.[gameId]
            let currentGame2 = gameStorage2.[gameId] 
            let currentplayer = match playerId with
                                            | p when p = currentGame.Player1 -> currentGame2.PlayerOne
                                            | p when p = currentGame.Player2 -> currentGame2.PlayerTwo//todo check for _
            let boolfirst = match playerId with //yea a mess
                                            | p when p = currentGame.Player1 -> true
                                            | p when p = currentGame.Player2 -> false
            placeBoat2 a currentplayer gameId boolfirst


    // Join game
    let joinGame gameId playerId =
        match gameStorage.ContainsKey(gameId) with
        | true ->
//            let currentGame2 = gameStorage2.[gameId] //pas nécéssaire ici vu qu'on initialise à la creation
//            Some {
//                GameServerOnly.GameId = currentGame2.GameId
//                GameServerOnly.PlayerOne = {  aPlayerBoats = DbBoats.Boats; aPlayerGrid = DbBoats.BoatGrid }
//                GameServerOnly.PlayerTwo = {  aPlayerBoats = DbBoats.Boats; aPlayerGrid = DbBoats.BoatGrid }
//            } |> ignore
            let currentGame = gameStorage.[gameId]
            Some {
                Id = currentGame.Id
                Player1 = currentGame.Player1
                Player2 = playerId
                State = GameState.Preparation
                Current = currentGame.Current
                Winner = currentGame.Winner
            } 
        | false -> None

    // Create game
    let createGame game =
        let id = gameStorage.Values.Count + 1
        let newGame = {
            Id = id
            Player1 = game.Player1
            Player2 = game.Player2
            State = GameState.WaitingForPlayer
            Current = -1
            Winner = -1
        }
        let newGame2 = {
                GameServerOnly.GameId = id
                GameServerOnly.PlayerOne = {  aPlayerBoats = DbBoats.Boats; aPlayerGrid = DbBoats.BoatGrid }
                GameServerOnly.PlayerTwo = {  aPlayerBoats = DbBoats.Boats; aPlayerGrid = DbBoats.BoatGrid }
            }
        gameStorage2.Add(id, newGame2);
        gameStorage.Add(id, newGame)//todo : get error!
        Errors.OptionLike.Some  newGame

    // Update game by id
    let updateGameById gameId (gameToBeUpdated:Game) =
        match gameStorage.ContainsKey(gameId) with
        | true ->
            let updatedGame = {
                Id = gameId
                Player1 = gameToBeUpdated.Player1
                Player2 = gameToBeUpdated.Player2
                State = gameToBeUpdated.State
                Current = gameToBeUpdated.Current
                Winner = gameToBeUpdated.Winner
            }
            gameStorage.[gameId] <- updatedGame
            Errors.OptionLike.Some updatedGame
        | false -> Errors.OptionLike.Error Errors.Errors.UnknownGame

    // Update game
    let updateGame gameToBeUpdated =
        updateGameById gameToBeUpdated.Id gameToBeUpdated

    // Delete game
    let deleteGame gameId =
        gameStorage2.Remove(gameId) |> ignore
        gameStorage.Remove(gameId) |> ignore

    // Game exists
    let isGameExists =
        gameStorage.ContainsKey