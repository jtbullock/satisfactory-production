// Learn more about F# at http://fsharp.org

open System
open SatisfactoryProductionLib
open System.Collections.Generic

[<EntryPoint>]
let main argv =
    printfn "Satisfactory Production Target Calculator"

    //let result = Production.determineRecipeDependencies MaterialRecipes.reinforcedIronPlate 30.0

    let requirements : MaterialRecipes.MaterialRecipe = 
       { OutputMaterial = "Production Targets"
         Machine = ""
         Output = 0.0
         MaterialDependencies = [
            { Material = MaterialIds.SmartPlate; Amount = 2.0 }
            { Material = MaterialIds.ModularFrame; Amount = 2.0 }
            { Material = MaterialIds.Rotor; Amount = 6.0 }
         ] }

    let result = Production.determineRecipeDependencies requirements 1.0

    printfn "----Generated dependency tree----"
    printfn "%A" result

    let aggResult = Production.buildProductionLevels result
    let resultWithMachies = Production.determineMachineRequirements aggResult

    printfn "----Generated dependency aggregates---"
    printfn "%-22s %5s %10s %14s %12s" "Material" "Tier" "Amount" "Machine" "#Machines"

    resultWithMachies
    |> List.iter (fun item -> printfn "%-22s %5i %10.1f %14s %12.0f" item.Material item.Level item.Amount item.Machine item.NumberOfMachines)

    0 // return an integer exit code


    // File format
    //,
    //{
    //  "OutputMaterial": "",
    //  "Machine": "",
    //  "Output":,
    //  "MaterialDependencies": [
    //    {
    //      "Material": "",
    //      "Amount":
    //    }
    //  ]
    //}