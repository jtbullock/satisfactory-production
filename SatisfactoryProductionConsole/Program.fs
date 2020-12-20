// Learn more about F# at http://fsharp.org

open System
open SatisfactoryProductionLib
open System.Collections.Generic

[<EntryPoint>]
let main argv =
    printfn "Satisfactory Production Target Calculator"

    //let result = Production.determineRecipeDependencies MaterialRecipes.reinforcedIronPlate 30.0

    let result = Production.determineRecipeDependencies MaterialRecipes.smartPlate 30.0

    printfn "----Generated dependency tree----"
    printfn "%A" result

    let aggResult = Production.buildProductionLevels result

    printfn "----Generated dependency aggregates---"
    printfn "%-22s %5s %10s" "Material" "Tier" "Amount"

    aggResult
    |> List.iter (fun item -> printfn "%-22s %5i %10.1f" item.Material item.Level item.Amount )

    0 // return an integer exit code
