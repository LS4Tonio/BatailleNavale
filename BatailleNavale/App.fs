module App

open Suave                 // always open suave
open Suave.Web                 // always open suave
open BatailleNavale.Rest
open BatailleNavale.db
open Suave.Successful

[<EntryPoint>]
let main argv =
    let userWebPart = rest "users" {
        GetAll = Db.getUsers
        Create = Db.createUser
        Update = Db.updateUser
    }

    let defaultWebPart = OK "Hello World"

    startWebServer defaultConfig (choose [userWebPart; defaultWebPart])
    0