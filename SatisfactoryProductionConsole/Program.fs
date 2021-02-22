open SatisfactoryProductionLib

let generateReport requirements recipes =
    printfn "----Generated dependency aggregates---"
    printfn "%-22s %5s %10s %17s" "Material" "Tier" "Amount" "Machines"

    Production.determineRecipeDependencies recipes requirements 1.0
    |> Production.buildProductionLevels
    |> Production.determineMachineRequirements recipes
    |> List.iter (fun item -> printfn "%-22s %5i %10.1f %14s x%.1f" item.Material item.Level item.Amount item.Machine item.NumberOfMachines)

[<EntryPoint>]
let main argv =
    printfn "Satisfactory Production Target Calculator"

    //let result = Production.determineRecipeDependencies MaterialRecipes.reinforcedIronPlate 30.0

    //let requirements : MaterialRecipes.MaterialRecipe = 
    //   { OutputMaterial = "Production Targets"
    //     Machine = ""
    //     Output = 0.0
    //     MaterialDependencies = [
    //        { Material = MaterialIds.SmartPlate; Amount = 2.0 }
    //        { Material = "VERSATILE_FRAMEWORK"; Amount = 5.0 }
    //        { Material = "AUTO_WIRING"; Amount = 2.5 }
    //     ] }

    let requirements : MaterialRecipes.MaterialRecipe = 
       { OutputMaterial = "Production Targets"
         Machine = ""
         Output = 0.0
         MaterialDependencies = [
            { Material = "SCIENCE_LOGISTICS"; Amount = 1.0 }
         ] }

    match MaterialRecipes.loadRecipes "factorio_recipes.json" with
    | Ok recipes -> generateReport requirements recipes
    | Error error -> printfn "Error loading JSON file: %s" error.Message

    0 // return an integer exit code

