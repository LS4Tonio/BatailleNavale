﻿namespace BatailleNavale.db.boats

type Coordinate = {
    X: int
    Y: int
}

type Boat = {
    Id: int
    Name: string
    Length: int
    Coordiates: List<Coordinate>
}

module DbBoats =
    let private carrier = {
        Id = 1
        Name = "Carrier"
        Length = 5
        Coordiates = []
    }
    let private battleship = {
        Id = 2
        Name = "Battleship"
        Length = 4
        Coordiates = []
    }
    let private cruiser = {
        Id = 3
        Name = "Cruiser"
        Length = 3
        Coordiates = []
    }
    let private submarine = {
        Id = 4
        Name = "Submarine"
        Length = 3
        Coordiates = []
    }
    let private destroyer = {
        Id = 5
        Name = "Destroyer"
        Length = 2
        Coordiates = []
    }

    let private defaultBoats = [carrier; battleship; cruiser; submarine; destroyer]

    // Get all boats
    let getBoats () =
        defaultBoats

    // Get boat
    let getBoat id =
        match List.find (fun b -> b.Id = id) defaultBoats with
        | b -> Some b
        | _ -> None