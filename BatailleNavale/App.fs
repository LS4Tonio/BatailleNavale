module App

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
    
    let notImpemented2 (a:'a):Errors.OptionLike<'a> = 
         Errors.OptionLike.Error Errors.NotImplemented
    let notImpemented3 (b:int) (a:'a):Errors.OptionLike<'a> = 
         Errors.OptionLike.Error Errors.NotImplemented
    let notImpemented4 (b:int) :Errors.OptionLike<SimpleBoat> =
         Errors.OptionLike.Error Errors.NotImplemented
    let notImpemented5 (b:int) = 
         Option.None
    let notImpemented6 (b:int) =
         Option.None |> ignore


    let boatWebPart = rest "placeboat" {
        GetAll = DbBoats.getBoats
        Create = DbBoats.placeBoat
        Update = DbBoats.updateBoat
        Delete = notImpemented6 //(DbBoats.deleteBoat |>  Errors.OptionLikeToOption)
        GetById = notImpemented5 //(DbBoats.getBoat |>  Errors.OptionLikeToOption)
        UpdateById = notImpemented3 //
        IsExists = DbGames.isGameExists
    }
    
    let boatWebPart2 = rest "defaultstartboats" {
        GetAll = DbBoats.getdefaultboatlistinformation
        Create = notImpemented2
        Update = notImpemented2
        Delete = notImpemented6 
        GetById = notImpemented5 
        UpdateById = notImpemented3 
        IsExists = DbGames.isGameExists
    }

    startWebServer config (choose [userWebPart; gameWebPart; boatWebPart; boatWebPart2; indexWebPart])

    //main must end with int
    0