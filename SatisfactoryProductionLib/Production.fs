namespace SatisfactoryProductionLib
open System.Collections.Generic

module MaterialIds =
    let IronOre = "IRON_ORE"
    let IronIngot = "IRON_INGOT"
    let IronPlate = "IRON_PLATE"
    let IronRod = "IRON_ROD"
    let Screw = "SCREW"
    let ReinforcedIronPlate = "REINFORCED_IRON_PLATE"

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

    let recipes = new Dictionary<string, MaterialRecipe>()
    recipes.Add(MaterialIds.IronIngot, ironIngot)
    recipes.Add(MaterialIds.IronPlate, ironPlate)
    recipes.Add(MaterialIds.IronRod, ironRod)
    recipes.Add(MaterialIds.Screw, screw)
    recipes.Add(MaterialIds.ReinforcedIronPlate, reinforcedIronPlate)

module Production =
    type MaterialDependencyReport =
        { Material: string
          Amount: float
          Dependencies: list<MaterialDependencyReport> }
    
    let rec determineRecipeDependencies (recipe: MaterialRecipes.MaterialRecipe) (amount: float) : list<MaterialDependencyReport> = 
        recipe.MaterialDependencies
        |> List.map ( fun dependency ->
            { Material = dependency.Material
              Amount = dependency.Amount * amount
              Dependencies = if (MaterialRecipes.recipes.ContainsKey dependency.Material) then determineRecipeDependencies MaterialRecipes.recipes.[dependency.Material] (dependency.Amount * amount) else [] } )

    type ProductionItem =
        { Material: string
          Amount: float
          Level: int }

    let updateAmount material amount listItem =
        if( listItem.Material = material ) then {listItem with Amount = listItem.Amount + amount} else listItem

    let rec buildProductionLevelsRec level productAggregates dependency =
        let agg = List.fold (buildProductionLevelsRec ( level + 1 ) ) productAggregates dependency.Dependencies

        let result = List.tryFind (fun a -> a.Material = dependency.Material) agg

        match result with
        | Some(x) -> List.map (updateAmount dependency.Material dependency.Amount) agg
        | None -> { Material = dependency.Material; Amount = dependency.Amount; Level = level } :: agg

    let buildProductionLevels (dependencies: list<MaterialDependencyReport>) =
        List.fold (buildProductionLevelsRec 0 ) List.empty dependencies


        

// For each dependency...


// [
//   [ { Material = "IRON_ORE", Amount = 12.0 } ];
//   [ { Material = "IRON_INGOT", Amount = 12.0 } ];
//   [ { Material = "IRON_ROD", Amount = 3.0 } ];
//   [ { Material = "SCREW", Amount = 12.0 }; { Material = "IRON_PLATE", Amount = 9 } ];
// ]

// [ 
//


// [{ Material = "IRON_PLATE"
// Amount = 6.0
// Dependencies = [{ Material = "IRON_INGOT"
//                   Amount = 9.0
//                   Dependencies = [{ Material = "IRON_ORE"
//                                     Amount = 9.0
//                                     Dependencies = [] }] }] };
//  { Material = "SCREW"
// Amount = 12.0
// Dependencies =
//               [{ Material = "IRON_ROD"
//                  Amount = 3.0
//                  Dependencies = [{ Material = "IRON_INGOT"
//                                    Amount = 3.0
//                                    Dependencies = [{ Material = "IRON_ORE"
//                                                      Amount = 3.0
//                                                      Dependencies = [] }] }] }] }]


// Describe deps for 2 iron plate
//var ironPlateDeps =   
//[
//	{
//		material: IRON_INGOT
//		amount: 3
//		dependencies: [
//			{
//				material: IRON_ORE
//				amount: 3
//				dependencies: []
//			}
//		]
//	}
//]