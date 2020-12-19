namespace SatisfactoryProductionLib

module Materials =
    
    let IRON_ORE = "IRON_ORE"
    let IRON_INGOT = "IRON_INGOT"
    let IRON_PLATE = "IRON_PLATE"

    type Material = 
        { id: string
          name: string
          isNaturallyOcurring: bool }

    let ironOreMaterial = 
        { id = IRON_ORE
          name = "Iron Ore"
          isNaturallyOcurring = true }

    let ironIngotMaterial = 
        { id = IRON_INGOT
          name = "Iron Ingot"
          isNaturallyOcurring = false }

module Production =

    type MaterialDependency = { material: string; amount: int }

    type MaterialRecipe = 
        { outputMaterial: string
          outputCount: int
          materialDependencies: list<MaterialDependency> }

    let ironIngotRecipe = {
        outputMaterial = Materials.IRON_INGOT
        outputCount = 1
        materialDependencies = [ { material = Materials.IRON_ORE; amount = 1  } ]
    }

    let determineRecipeDependencies (recipe: MaterialRecipe) amount = 
        recipe.materialDependencies
        |> List.map ( fun dependency -> { dependency with amount = dependency.amount * amount } ) 
        