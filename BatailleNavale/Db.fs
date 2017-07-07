namespace BatailleNavale.db

open System.Collections.Generic

type User = {
    Id: int
    Username: string
}

module Db =
    let private userStorage = new Dictionary<int, User>()
    let getUsers () =
        userStorage.Values |> Seq.map (fun p -> p)

    let createUser user =
        let id = userStorage.Values.Count + 1
        let newUser = {
            Id = id
            Username = user.Username
        }
        userStorage.Add(id, newUser)
        newUser