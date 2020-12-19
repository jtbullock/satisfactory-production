namespace SatisfactoryProductionLib
open System.Collections.Generic

module Materials =
    
    let IronOreId = "IRON_ORE"
    let IronIngotId = "IRON_INGOT"
    let IronPlateId = "IRON_PLATE"

    type Material = 
        { Id: string
          Name: string
          IsNaturallyOcurring: bool }

    let ironOreMaterial = 
        { Id = IronOreId
          Name = "Iron Ore"
          IsNaturallyOcurring = true }

    let ironIngotMaterial = 
        { Id = IronIngotId
          Name = "Iron Ingot"
          IsNaturallyOcurring = false }

module Production =

    type MaterialDependency = { Material: string; Amount: int }

    type MaterialRecipe = 
        { OutputMaterial: string
          OutputCount: int
          MaterialDependencies: list<MaterialDependency> }

    let ironIngotRecipe = 
        { OutputMaterial = Materials.IronIngotId
          OutputCount = 1
          MaterialDependencies = [ { Material = Materials.IronOreId; Amount = 1  } ] }

    let ironPlateRecipe = 
        { OutputMaterial = Materials.IronPlateId
          OutputCount = 2
          MaterialDependencies = [ { Material = Materials.IronIngotId; Amount = 3 } ] }

    let recipes = new Dictionary<string, MaterialRecipe>()
    recipes.Add(Materials.IronIngotId, ironIngotRecipe)
    
    let determineRecipeDependencies (recipe: MaterialRecipe) amount = 
        recipe.MaterialDependencies
        |> List.map ( fun dependency -> { dependency with Amount = dependency.Amount * amount } ) 

        
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