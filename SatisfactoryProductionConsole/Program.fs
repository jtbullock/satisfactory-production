﻿// Learn more about F# at http://fsharp.org

open System
open SatisfactoryProductionLib
open System.Collections.Generic

[<EntryPoint>]
let main argv =
    printfn "Satisfactory Production Target Calculator"

    let result = Production.determineRecipeDependencies MaterialRecipes.reinforcedIronPlate 1.0

    printfn "----Generated dependency tree----"
    printfn "%A" result

    let aggResult = Production.buildProductionLevels result

    printfn "----Generated dependency aggregates---"
    aggResult
    |> List.iter (fun item -> printfn "%s - %i - %f" item.Material item.Level item.Amount )

    0 // return an integer exit code
