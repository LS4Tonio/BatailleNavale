﻿module App

open Suave
open Suave.Web
open Suave.RequestErrors
open Suave.Filters
open Suave.Operators
open System.IO
open BatailleNavale.Rest
open BatailleNavale.db.users
open BatailleNavale.db.games
open BatailleNavale.db.boats

[<EntryPoint>]
let main argv =
    let indexWebPart = choose [
        GET >=> path "/" >=> Files.file "index.html"
        GET >=> Files.browseHome
        RequestErrors.NOT_FOUND "Page not found"
    ]
    let config = {
        defaultConfig with homeFolder = Some (Path.GetFullPath "./")
    }

    let userWebPart = rest "users" {
       GetAll = DbUsers.getUsers
       Create = DbUsers.createUser
       Update = DbUsers.updateUser
       Delete = DbUsers.deleteUser
       GetById = DbUsers.getUser
       UpdateById = DbUsers.updateUserById
       IsExists = DbUsers.isUserExists
    }
    let gameWebPart = rest "games" {
        GetAll = DbGames.getAll
        Create = DbGames.createGame
        Update = DbGames.updateGame
        Delete = DbGames.deleteGame
        GetById = DbGames.getById
        UpdateById = DbGames.updateGameById
        IsExists = DbGames.isGameExists
    }
    let boatWebPart = rest "placeboat" {
        GetAll = DbBoats.getBoats
        Create = DbBoats.placeBoat
        Update = Rules.getAll
        Delete = Rules.getAll
        GetById = DbBoats.getBoat
        UpdateById = DbGames.updateGameById
        IsExists = DbGames.isGameExists
    }

    startWebServer config (choose [userWebPart; gameWebPart; indexWebPart])

    //main must end with int
    0