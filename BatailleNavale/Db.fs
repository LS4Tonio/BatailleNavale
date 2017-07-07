namespace BatailleNavale.db

open System.Collections.Generic

type User = {
    Id: int
    Username: string
}

module Db =
    let private userStorage = new Dictionary<int, User>()

    // Get all users
    let getUsers () =
        userStorage.Values |> Seq.map (fun p -> p)

    // Create user
    let createUser user =
        let id = userStorage.Values.Count + 1
        let newUser = {
            Id = id
            Username = user.Username
        }
        userStorage.Add(id, newUser)
        newUser

    // Update user by id
    let updateUserById userId userToBeUpdated =
        if userStorage.ContainsKey(userId) then
            let updatedUser = {
                Id = userId
                Username = userToBeUpdated.Username
            }
            userStorage.[userId] <- updatedUser
            Some updatedUser
        else
            None

    // Update user
    let updateUser userToBeUpdated =
        updateUserById userToBeUpdated.Id userToBeUpdated