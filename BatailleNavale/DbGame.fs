﻿namespace BatailleNavale.db.games

open System.Collections.Generic

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

module DbGames =
    let private gameStorage = new Dictionary<int, Game>()

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

    // Join game
    let joinGame gameId playerId =
        match gameStorage.ContainsKey(gameId) with
        | true ->
            let currentGame = gameStorage.[gameId]
            Some {
                Id = currentGame.Id
                Player1 = currentGame.Player1
                Player2 = currentGame.Player2
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
        gameStorage.Add(id, newGame)
        newGame

    // Update game by id
    let updateGameById gameId gameToBeUpdated =
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
            Some updatedGame
        | false -> None

    // Update game
    let updateGame gameToBeUpdated =
        updateGameById gameToBeUpdated.Id gameToBeUpdated

    // Delete game
    let deleteGame gameId =
        gameStorage.Remove(gameId) |> ignore

    // Game exists
    let isGameExists =
        gameStorage.ContainsKey