namespace FSharp.Finance.Personal.Tests

open Xunit
open FsUnit.Xunit

open FSharp.Finance.Personal.EquipmentFinance.Depreciation.Common

module DepreciationCommonTests =

    [<Fact>]
    let ``Rounding currency works correctly`` () =
        Rounding.roundCurrency 12.345m |> should equal 12.35m
        Rounding.roundCurrency 12.344m |> should equal 12.34m
        Rounding.roundCurrency 12.346m |> should equal 12.35m

    [<Fact>]
    let ``Rounding to places works correctly`` () =
        Rounding.roundToPlaces 3 12.3456m |> should equal 12.346m
        Rounding.roundToPlaces 1 12.34m |> should equal 12.3m
        Rounding.roundToPlaces 0 12.7m |> should equal 13m

    [<Fact>]
    let ``Rounding percentage works correctly`` () =
        Rounding.roundPercentage 0.123456m |> should equal 0.1235m
        Rounding.roundPercentage 0.12m |> should equal 0.12m

    [<Fact>]
    let ``Validate positive amount accepts positive values`` () =
        Validation.validatePositiveAmount 100m "test" // Should not throw

    [<Fact>]
    let ``Validate positive amount rejects zero and negative`` () =
        (fun () -> Validation.validatePositiveAmount 0m "test") |> should throw typeof<System.Exception>
        (fun () -> Validation.validatePositiveAmount -10m "test") |> should throw typeof<System.Exception>

    [<Fact>]
    let ``Validate percentage accepts valid range`` () =
        Validation.validatePercentage 0m "test" // Should not throw
        Validation.validatePercentage 0.5m "test" // Should not throw
        Validation.validatePercentage 1m "test" // Should not throw

    [<Fact>]
    let ``Validate percentage rejects invalid range`` () =
        (fun () -> Validation.validatePercentage -0.1m "test") |> should throw typeof<System.Exception>
        (fun () -> Validation.validatePercentage 1.1m "test") |> should throw typeof<System.Exception>

    [<Fact>]
    let ``Calculate remaining value works`` () =
        Calculations.calculateRemainingValue 10000m 3000m |> should equal 7000m
        Calculations.calculateRemainingValue 5000m 5000m |> should equal 0m

    [<Fact>]
    let ``Apply rate works correctly`` () =
        Calculations.applyRate 1000m 0.18m |> should equal 180m
        Calculations.applyRate 5000m 0.06m |> should equal 300m

    [<Fact>]
    let ``Calculate cumulative works`` () =
        let amounts = [100m; 200m; 150m]
        let cumulative = Calculations.calculateCumulative amounts
        
        cumulative |> should equal [100m; 300m; 450m]

    [<Fact>]
    let ``Cap depreciation at cost works`` () =
        // Normal case - no capping needed
        Calculations.capDepreciationAtCost 10000m 1000m 5000m |> should equal 1000m
        
        // Capping needed - proposed exceeds remaining
        Calculations.capDepreciationAtCost 10000m 6000m 5000m |> should equal 5000m
        
        // Edge case - exactly at limit
        Calculations.capDepreciationAtCost 10000m 5000m 5000m |> should equal 5000m

    [<Fact>]
    let ``Format currency works`` () =
        Formatting.formatCurrency "£" 1234.56m |> should equal "£1,234.56"
        Formatting.formatCurrency "$" 999.99m |> should equal "$999.99"

    [<Fact>]
    let ``Format percentage works`` () =
        Formatting.formatPercentage 2 0.1234m |> should equal "12.34%"
        Formatting.formatPercentage 1 0.18m |> should equal "18.0%"

    [<Fact>]
    let ``Format period summary works`` () =
        let period = {
            Period = 1
            Amount = 1000m
            CumulativeAmount = 1000m
            RemainingValue = 4000m
        }
        
        let summary = Formatting.formatPeriodSummary period "$"
        summary |> should contain "Period 1"
        summary |> should contain "$1,000.00"
        summary |> should contain "$4,000.00"

    [<Fact>]
    let ``Educational disclaimer is not empty`` () =
        Disclaimers.EducationalDisclaimer |> should not' (be EmptyString)
        Disclaimers.UKSpecificDisclaimer |> should not' (be EmptyString)
        Disclaimers.USSpecificDisclaimer |> should not' (be EmptyString)