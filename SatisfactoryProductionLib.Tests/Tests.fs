namespace SatisfactoryProductionLib.Tests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open SatisfactoryProductionLib

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.TestEvenSequence() = 
        let expected = Seq.empty<int> |> Seq.toList
        let actual = TestMath.squaresOfOdds [2; 4; 6; 8; 10]
        Assert.AreEqual(expected, actual)

    [<TestMethod>]
    member this.TestOnesAndEvens() =
        let expected = [1; 1; 1; 1]
        let actual = TestMath.squaresOfOdds [2; 1; 4; 1; 6; 1; 8; 1; 10]
        Assert.AreEqual(expected, actual)

    [<TestMethod>]
    member this.TestSquaresOfOdds() = 
        let expected = [1; 9; 25; 49; 81]
        let actual = TestMath.squaresOfOdds [1; 2; 3; 4; 5; 6; 7; 9; 10]
        Assert.AreEqual(expected, actual)

[<TestClass>]
type ProductionTestClass () =

    [<TestMethod>]
    member this.TestGetRecipeDependencies() =
        let expected : list<Production.MaterialDependency> = [ { Material = Materials.IronOreId; Amount = 10 } ]
        let actual = Production.determineRecipeDependencies Production.ironIngotRecipe 10
        Assert.AreEqual(expected, actual)
