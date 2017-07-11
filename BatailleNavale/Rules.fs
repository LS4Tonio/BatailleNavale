module Rules
//list7 & resultList2 is the proper example


type Untruc = int

type Deuxtruc =
    | UnInt of Untruc
    | Empty

let returndeuxtruc p =
    match p with
    | a when a<10 -> Deuxtruc.UnInt p
    | _ -> Deuxtruc.Empty



let aggregateString s1 s2 = 
    match String.length s1, String.length s2 with
    | l1, l2 when l1>  l2 -> s1
    | _ -> s2

    
let something index2 index3 = 
      index2 * index3  
let printsomething s1 =
    let list = [["a";"absg"];["a";"ag"];["a";"agd"];["a";"agf"]]
    
    (* using init method *)
    let list5 = List.init 5 (fun index -> (index, index * index, index * index * index))
    let list6 = List.init 5 (fun index -> List.init ( something index 1))
    let list7 = List.init 5 (fun index -> List.init 3 ( fun index2 -> something index index2))
    printfn "The list (5): %A" list5
    printfn "The list: %A" list
    let resultList = List.collect (fun smthg -> printfn "The list (7): %A" smthg
                                                ["a"]
                                    )   list7 
    printfn "The list (6): %A" list6
    printfn "The list (7): %A" list7
    printfn "The resultList: %A" resultList

    let itochange1 = 1;
    let itochange2 = 2;
    let itochangeto = 100;
    let resultList4 =  list7 |> List.mapi  (fun i a -> a  ) 
    let resultList3 =  list7 |> List.mapi  (fun i a -> 
        match i with | itochange -> a
                     | _ -> a  ) 
                                                                                                                                
    let resultList2 = list7 |> List.mapi(fun i a -> 
        match i with 
        | index1 when index1 = itochange1 -> a |> List.mapi(fun i2 b -> 
            match i2 with  
                  | index2 when index2 = itochange2 -> itochangeto
                  | _ -> b)
        | _ -> a ) 

    printfn "The resultList2: %A" resultList2
    
    printfn "%s" (["a";"ab";"abc";"abcd"] |> List.reduce aggregateString)

