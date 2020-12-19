namespace SatisfactoryProductionLib

module Say =

    let hello name =
        printfn "Hello %s" name

module TestMath = 

    let private isOdd x = x % 2 <> 0
    let private square x = x * x

    let squaresOfOdds xs =
        xs
        |> Seq.filter isOdd 
        |> Seq.map square
        |> Seq.toList

