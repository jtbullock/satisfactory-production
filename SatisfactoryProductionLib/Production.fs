namespace SatisfactoryProductionLib
open System.Collections.Generic

module MaterialIds =
    let IronOre = "IRON_ORE"
    let IronIngot = "IRON_INGOT"
    let IronPlate = "IRON_PLATE"
    let IronRod = "IRON_ROD"
    let Screw = "SCREW"
    let ReinforcedIronPlate = "REINFORCED_IRON_PLATE"
    let Rotor = "ROTOR"
    let SmartPlate = "SMART_PLATE"
    let ModularFrame = "MODULAR_FRAME"

module MaterialRecipes =
    type MaterialDependency = { Material: string; Amount: float }
    
    type MaterialRecipe = 
        { OutputMaterial: string
          MaterialDependencies: list<MaterialDependency> }
    
    let ironIngot = 
        { OutputMaterial = MaterialIds.IronIngot
          MaterialDependencies = [ { Material = MaterialIds.IronOre; Amount = 1.0  } ] }
    
    let ironPlate = 
        { OutputMaterial = MaterialIds.IronPlate
          MaterialDependencies = [ { Material = MaterialIds.IronIngot; Amount = 1.5 } ] }

    let ironRod =
        { OutputMaterial = MaterialIds.IronRod
          MaterialDependencies = [ { Material = MaterialIds.IronIngot; Amount = 1.0 } ] }

    let screw =
        { OutputMaterial = MaterialIds.Screw
          MaterialDependencies = [ { Material = MaterialIds.IronRod; Amount = 0.25 } ] }

    let reinforcedIronPlate =
        { OutputMaterial = MaterialIds.ReinforcedIronPlate
          MaterialDependencies =
            [ { Material = MaterialIds.IronPlate; Amount = 6.0 }
              { Material = MaterialIds.Screw; Amount = 12.0 } ] }

    let rotor =
        { OutputMaterial = MaterialIds.Rotor
          MaterialDependencies =
            [ { Material = MaterialIds.IronRod; Amount = 5.0 }
              { Material = MaterialIds.Screw; Amount = 25.0 } ] }

    let smartPlate =
        { OutputMaterial = MaterialIds.SmartPlate
          MaterialDependencies =
            [ { Material = MaterialIds.ReinforcedIronPlate; Amount = 1.0 }
              { Material = MaterialIds.Rotor; Amount = 1.0 } ] }

    let modularFrame =
        { OutputMaterial = MaterialIds.ModularFrame
          MaterialDependencies =
            [ { Material = MaterialIds.ReinforcedIronPlate; Amount = 1.5 }
              { Material = MaterialIds.IronRod; Amount = 6.0 } ] }

    let recipes = new Dictionary<string, MaterialRecipe>()
    recipes.Add(MaterialIds.IronIngot, ironIngot)
    recipes.Add(MaterialIds.IronPlate, ironPlate)
    recipes.Add(MaterialIds.IronRod, ironRod)
    recipes.Add(MaterialIds.Screw, screw)
    recipes.Add(MaterialIds.ReinforcedIronPlate, reinforcedIronPlate)
    recipes.Add(MaterialIds.Rotor, rotor)
    recipes.Add(MaterialIds.SmartPlate, smartPlate)
    recipes.Add(MaterialIds.ModularFrame, modularFrame)

module Production =

    // ***** Dependency Tree ***** //
    type MaterialDependencyReport =
        { Material: string
          Amount: float
          Dependencies: list<MaterialDependencyReport> }
    
    let rec determineRecipeDependencies (recipe: MaterialRecipes.MaterialRecipe) (amount: float) : list<MaterialDependencyReport> = 
        recipe.MaterialDependencies
        |> List.map ( fun dependency ->

            let dependencies =
                if (MaterialRecipes.recipes.ContainsKey dependency.Material) then
                    determineRecipeDependencies MaterialRecipes.recipes.[dependency.Material] (dependency.Amount * amount)
                else
                    []

            { Material = dependency.Material
              Amount = dependency.Amount * amount
              Dependencies = dependencies } )

    // ***** Production Levels ***** //
    type ProductionItem =
        { Material: string
          Amount: float
          Level: int }

    let updateProductionItem listItem amount level =
        { listItem with Amount = listItem.Amount + amount; Level = if( level > listItem.Level ) then level else listItem.Level }

    let updateAmount material amount level listItem =
        if( listItem.Material = material ) then updateProductionItem listItem amount level else listItem

    let rec buildProductionLevelsRec level productAggregates dependency =
        let agg = List.fold (buildProductionLevelsRec ( level + 1 ) ) productAggregates dependency.Dependencies
       
        let result = List.tryFind (fun a -> a.Material = dependency.Material) agg

        match result with
        | Some(x) -> List.map (updateAmount dependency.Material dependency.Amount level) agg
        | None -> { Material = dependency.Material; Amount = dependency.Amount; Level = level } :: agg

    let buildProductionLevels (dependencies: list<MaterialDependencyReport>) =
        List.fold (buildProductionLevelsRec 0 ) List.empty dependencies
        |> List.sortBy (fun r -> r.Level)