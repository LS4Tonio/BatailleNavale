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