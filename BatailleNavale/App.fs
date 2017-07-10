module App

open Suave                      // always open suave
open Suave.Web                  // always open suave
open BatailleNavale.Rest
open BatailleNavale.db.users
open Suave.Successful

[<EntryPoint>]
let main argv =
    let userWebPart = rest "users" {
        GetAll = DbUsers.getUsers
        Create = DbUsers.createUser
        Update = DbUsers.updateUser
        Delete = DbUsers.deleteUser
        GetById = DbUsers.getUser
        UpdateById = DbUsers.updateUserById
        IsExists = DbUsers.isUserExists
    }

    let defaultWebPart = OK "Hello World"

    startWebServer defaultConfig (choose [userWebPart; defaultWebPart])
    0