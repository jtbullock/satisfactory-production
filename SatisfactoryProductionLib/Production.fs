namespace SatisfactoryProductionLib

module Json =

  open Newtonsoft.Json
    
  let serialize obj =
    JsonConvert.SerializeObject obj

  let deserialize<'a> str =
    try
      JsonConvert.DeserializeObject<'a> str
      |> Result.Ok
    with
      // catch all exceptions and convert to Result
      | ex -> Result.Error ex  

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

//module Machines =
//    let Smelter = "SMELTER"
//    let Constructor = "CONSTRUCTOR"
//    let Assembler = "ASSEMBLER"

module MaterialRecipes =
    type MaterialDependency = { Material: string; Amount: float }
    
    type MaterialRecipe = 
        { OutputMaterial: string
          MaterialDependencies: list<MaterialDependency>
          Machine: string
          Output: float }

    //type loadRecipesResult =
    //    | Success of Map<string, MaterialRecipe>
    //    | FileReadError
    //    | JsonParseError

    let loadRecipes sourceFile = 
        System.IO.File.ReadAllText sourceFile 
        |> Json.deserialize<list<MaterialRecipe>>
        |> Result.bind (fun list -> List.map (fun r -> r.OutputMaterial, r) list |> Map.ofList |> Ok )

module Production =

    open MaterialRecipes

    // ***** Dependency Tree ***** //
    type MaterialDependencyReport =
        { Material: string
          Amount: float
          Dependencies: list<MaterialDependencyReport> }
    
    let rec determineRecipeDependencies (recipes: Map<string, MaterialRecipe>)
        (recipe: MaterialRecipe) amount : list<MaterialDependencyReport> = 
            recipe.MaterialDependencies
            |> List.map ( fun dependency ->
                { Material = dependency.Material
                  Amount = dependency.Amount * amount
                  Dependencies =
                      match recipes.TryFind dependency.Material with
                      | Some recipe -> determineRecipeDependencies recipes recipe (dependency.Amount * amount)
                      | None -> []
                })

    // ***** Production Levels ***** //
    type ProductionItem =
        { Material: string
          Amount: float
          Level: int }

    let private updateProductionItem listItem amount level =
        { Material = listItem.Material
          Amount = listItem.Amount + amount
          Level =
            match level with
            | _ when level > listItem.Level -> level
            | _ -> listItem.Level }

    let private updateAmount material amount level listItem =
        match listItem.Material with
        | _ when listItem.Material = material -> updateProductionItem listItem amount level
        | _ -> listItem

    let rec private buildProductionLevelsRec level productAggregates dependency =
        let agg = List.fold (buildProductionLevelsRec ( level + 1 ) ) productAggregates dependency.Dependencies
       
        List.tryFind (fun a -> a.Material = dependency.Material) agg
        |> function
           | Some(_) -> List.map (updateAmount dependency.Material dependency.Amount level) agg
           | None -> { Material = dependency.Material; Amount = dependency.Amount; Level = level } :: agg

    let buildProductionLevels (dependencies: list<MaterialDependencyReport>) =
        List.fold (buildProductionLevelsRec 0 ) List.empty dependencies
        |> List.sortBy (fun r -> r.Level)

    // ***** Machine Requirements ***** //
    type ProductionItemWithMachineRequirements = 
        { Material: string
          Amount: float
          Level: int
          Machine: string
          NumberOfMachines: float }

    let private determineMachineRequirementsForItem (recipes: Map<string, MaterialRecipes.MaterialRecipe>) (item: ProductionItem) =

        if(recipes.ContainsKey item.Material) then
            let recipe = recipes.[item.Material]

            { Material = item.Material
              Amount = item.Amount
              Level = item.Level
              Machine = recipe.Machine
              NumberOfMachines = ceil (item.Amount / float recipe.Output) }
         else
            { Material = item.Material
              Amount = item.Amount
              Level = item.Level
              Machine = ""
              NumberOfMachines = 0.0 }

    let determineMachineRequirements (recipes: Map<string, MaterialRecipes.MaterialRecipe>) (productionItems: list<ProductionItem>) =
        productionItems
        |> List.map ( determineMachineRequirementsForItem recipes )