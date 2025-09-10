namespace FSharp.Finance.Personal.Tests

open Xunit
open FsUnit.Xunit

open FSharp.Finance.Personal.EquipmentFinance.Depreciation.US_MACRS

module USMacrsTests =

    [<Fact>]
    let ``Asset class recovery periods are correct`` () =
        Calculations.getRecoveryPeriod Types.AssetClass.ThreeYear |> should equal 3
        Calculations.getRecoveryPeriod Types.AssetClass.FiveYear |> should equal 5
        Calculations.getRecoveryPeriod Types.AssetClass.SevenYear |> should equal 7
        Calculations.getRecoveryPeriod Types.AssetClass.TenYear |> should equal 10
        Calculations.getRecoveryPeriod Types.AssetClass.FifteenYear |> should equal 15
        Calculations.getRecoveryPeriod Types.AssetClass.TwentyYear |> should equal 20

    [<Fact>]
    let ``Five-year property percentages are available`` () =
        let percentages = Tables.getDepreciationPercentages Types.AssetClass.FiveYear
        
        percentages.Length |> should equal 6
        percentages.[0] |> should equal 20.00m
        percentages.[1] |> should equal 32.00m
        percentages.[2] |> should equal 19.20m
        percentages.[3] |> should equal 11.52m
        percentages.[4] |> should equal 11.52m
        percentages.[5] |> should equal 5.76m

    [<Fact>]
    let ``Seven-year property percentages are available`` () =
        let percentages = Tables.getDepreciationPercentages Types.AssetClass.SevenYear
        
        percentages.Length |> should equal 8
        percentages.[0] |> should equal 14.29m
        percentages.[1] |> should equal 24.49m

    [<Fact>]
    let ``Five-year asset depreciation schedule is correct`` () =
        let config = {
            OriginalBasis = 10_000m
            AssetClass = Types.AssetClass.FiveYear
            Convention = "Half-Year"
        }
        
        let schedule = Calculations.generateSchedule config
        
        schedule.Length |> should equal 6
        
        // Year 1: 20% of $10,000 = $2,000
        let year1 = schedule |> List.head
        year1.Year |> should equal 1
        year1.DepreciationRate |> should equal 0.20m
        year1.DepreciationAmount |> should equal 2_000m
        year1.AccumulatedDepreciation |> should equal 2_000m
        year1.BookValue |> should equal 8_000m
        
        // Year 2: 32% of $10,000 = $3,200
        let year2 = schedule.[1]
        year2.Year |> should equal 2
        year2.DepreciationRate |> should equal 0.32m
        year2.DepreciationAmount |> should equal 3_200m
        year2.AccumulatedDepreciation |> should equal 5_200m
        year2.BookValue |> should equal 4_800m

    [<Fact>]
    let ``Three-year asset depreciation schedule is correct`` () =
        let config = {
            OriginalBasis = 3_000m
            AssetClass = Types.AssetClass.ThreeYear
            Convention = "Half-Year"
        }
        
        let schedule = Calculations.generateSchedule config
        
        schedule.Length |> should equal 4
        
        // Year 1: 33.33% of $3,000 ≈ $999.90
        let year1 = schedule |> List.head
        year1.Year |> should equal 1
        year1.DepreciationAmount |> should (equalWithin 0.01) 999.90m
        
        // Final year should have minimal book value
        let lastYear = schedule |> List.last
        lastYear.BookValue |> should be (lessThan 300m) // Most should be depreciated

    [<Fact>]
    let ``Total depreciation equals original basis`` () =
        let config = {
            OriginalBasis = 5_000m
            AssetClass = Types.AssetClass.SevenYear
            Convention = "Half-Year"
        }
        
        let schedule = Calculations.generateSchedule config
        
        let totalDepreciation = 
            schedule 
            |> List.sumBy (fun year -> year.DepreciationAmount)
        
        // Total should equal original basis (within rounding tolerance)
        totalDepreciation |> should (equalWithin 0.50m) config.OriginalBasis

    [<Fact>]
    let ``Example computer schedule works`` () =
        let schedule = Examples.exampleComputerSchedule ()
        
        schedule |> should not' (be Empty)
        schedule.Length |> should equal 6
        
        let year1 = schedule |> List.head
        year1.Year |> should equal 1
        year1.DepreciationAmount |> should equal 2_000m // 20% of $10,000

    [<Fact>]
    let ``Example furniture schedule works`` () =
        let schedule = Examples.exampleFurnitureSchedule ()
        
        schedule |> should not' (be Empty)
        schedule.Length |> should equal 8 // 7-year property has 8 years
        
        let year1 = schedule |> List.head
        year1.Year |> should equal 1
        year1.DepreciationAmount |> should equal 714.50m // 14.29% of $5,000

    [<Fact>]
    let ``Example equipment schedule works`` () =
        let schedule = Examples.exampleEquipmentSchedule ()
        
        schedule |> should not' (be Empty)
        schedule.Length |> should equal 8
        
        let year1 = schedule |> List.head
        year1.Year |> should equal 1
        year1.DepreciationAmount |> should equal 3_572.50m // 14.29% of $25,000

    [<Fact>]
    let ``Book value decreases each year`` () =
        let config = {
            OriginalBasis = 15_000m
            AssetClass = Types.AssetClass.FiveYear
            Convention = "Half-Year"
        }
        
        let schedule = Calculations.generateSchedule config
        
        // Book value should decrease each year
        let bookValues = schedule |> List.map (fun year -> year.BookValue)
        
        bookValues 
        |> List.pairwise
        |> List.iter (fun (prev, curr) -> curr |> should be (lessThan prev))

    [<Fact>]
    let ``Accumulated depreciation increases each year`` () =
        let config = {
            OriginalBasis = 8_000m
            AssetClass = Types.AssetClass.ThreeYear
            Convention = "Half-Year"
        }
        
        let schedule = Calculations.generateSchedule config
        
        // Accumulated depreciation should increase each year
        let accumulatedValues = schedule |> List.map (fun year -> year.AccumulatedDepreciation)
        
        accumulatedValues 
        |> List.pairwise
        |> List.iter (fun (prev, curr) -> curr |> should be (greaterThan prev))