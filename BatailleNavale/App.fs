module App

open Suave                      // always open suave
open Suave.Web                  // always open suave
open BatailleNavale.Rest
open BatailleNavale.db.users
open BatailleNavale.db.games
open Rules
open Suave.Successful
open Suave.RequestErrors
open Suave.Filters
open Suave.Operators
open System.IO

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

    //let defaultWebPart = OK "Hello World"

    //printsomething 1

    startWebServer config (choose [userWebPart; gameWebPart; indexWebPart])
    //main must end with int
    0