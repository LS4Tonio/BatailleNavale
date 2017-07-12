namespace BatailleNavale.db.users

open System.Collections.Generic

type User = {
    Id: int
    Username: string
}

module DbUsers =
    let private userStorage = new Dictionary<int, User>()

    let getUserstorage () = userStorage

    // Get all users
    let getUsers () =
        userStorage.Values |> Seq.map (fun u -> u)

    // Get user
    let getUser id =
        match userStorage.ContainsKey(id) with
        | true -> Some userStorage.[id]
        | false -> None

    // Create user
    let createUser user =
        let id = userStorage.Values.Count + 1
        let newUser = {
            Id = id
            Username = user.Username
        }
        userStorage.Add(id, newUser)
        Errors.OptionLike.Some newUser

    // Update user by id
    let updateUserById userId userToBeUpdated =
        match userStorage.ContainsKey(userId) with
        | true ->
            let updatedUser = {
                Id = userId
                Username = userToBeUpdated.Username
            }
            userStorage.[userId] <- updatedUser
            Errors.OptionLike.Some updatedUser
        | false -> Errors.OptionLike.Error Errors.UnknownUser

    // Update user
    let updateUser userToBeUpdated =
         updateUserById userToBeUpdated.Id userToBeUpdated 

    // Delete user
    let deleteUser userId =
        userStorage.Remove(userId) |> ignore

    // User exists
    let isUserExists =
        userStorage.ContainsKey