﻿module App

open Suave                      // always open suave
open Suave.Web                  // always open suave
open BatailleNavale.Rest
open BatailleNavale.db.users
open Rules
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

    printsomething 1

//    let myCfg =
//        { 
//            defaultConfig with
//             bindings = [ HttpBinding.createSimple HTTP "10.8.110.223" 8080 ]
//        }
//    startWebServer myCfg (choose [userWebPart; defaultWebPart])


    startWebServer defaultConfig (choose [userWebPart; defaultWebPart])
    //main must end with int
    0