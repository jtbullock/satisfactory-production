// Learn more about F# at http://fsharp.org

open System
open SatisfactoryProductionLib

[<EntryPoint>]
let main argv =
    printfn "Satisfactory Production Target Calculator"

    let result = Production.determineRecipeDependencies MaterialRecipes.reinforcedIronPlate 1.0

    printfn "%A" result

    0 // return an integer exit code
