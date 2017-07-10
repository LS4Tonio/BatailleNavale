namespace BatailleNavale.db.games

open System.Collections.Generic

type Game = {
    Id: int
    User1: int
    User2: int
}

module DbGames =
    let private gameStorage = new Dictionary<int, Game>()