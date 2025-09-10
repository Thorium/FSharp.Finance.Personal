(**
# Equipment Finance Depreciation Examples

This script demonstrates the usage of the Equipment Finance depreciation modules
for both UK Capital Allowances and US MACRS calculations.

## Disclaimer

These examples are for educational and analytical purposes only.
They are NOT tax advice and should not be used for actual tax calculations
without validation by qualified tax professionals.
*)

#r "nuget: FSharp.Finance.Personal"

open FSharp.Finance.Personal.EquipmentFinance.Depreciation.UK_CapitalAllowances
open FSharp.Finance.Personal.EquipmentFinance.Depreciation.US_MACRS

(**
## UK Capital Allowances Examples

### Example 1: Machinery in Main Pool (Small Asset)

A small piece of manufacturing equipment costing £25,000 in the main pool.
This will be fully claimed via AIA in year 1.
*)

let smallMachinery = {
    Types.Amount = 25_000m
    Types.Pool = Types.Pool.Main
    Types.Description = "Small manufacturing equipment"
}

printfn "=== UK Capital Allowances: Small Machinery ==="
printfn "Cost: £%M in main pool" smallMachinery.Amount
printfn ""

let smallSchedule = Calculations.scheduleDefault smallMachinery

printfn "Year\tAIA\t\tWDA\t\tTotal\t\tPool EOY"
smallSchedule 
|> List.iter (fun year ->
    printfn "%d\t£%M\t£%M\t£%M\t£%M" 
        year.Year 
        year.AnnualInvestmentAllowance 
        year.WritingDownAllowance 
        year.TotalAllowances 
        year.PoolValueEndOfYear)

(**
### Example 2: Large Equipment in Main Pool

A large piece of equipment costing £150,000 in the main pool.
This exceeds the AIA limit, so will use both AIA and WDA.
*)

printfn "\n=== UK Capital Allowances: Large Equipment ==="

let largeEquipment = {
    Types.Amount = 150_000m
    Types.Pool = Types.Pool.Main
    Types.Description = "Large manufacturing equipment"
}

let customConfig = {
    Types.Default with
        AnnualInvestmentAllowanceLimit = 100_000m
        MaxYears = 6
}

printfn "Cost: £%M in main pool" largeEquipment.Amount
printfn "AIA limit: £%M" customConfig.AnnualInvestmentAllowanceLimit
printfn ""

let largeSchedule = Calculations.generateSchedule customConfig largeEquipment

printfn "Year\tAIA\t\tWDA\t\tTotal\t\tPool EOY"
largeSchedule 
|> List.iter (fun year ->
    printfn "%d\t£%M\t£%M\t£%M\t£%M" 
        year.Year 
        year.AnnualInvestmentAllowance 
        year.WritingDownAllowance 
        year.TotalAllowances 
        year.PoolValueEndOfYear)

(**
### Example 3: Vehicle in Special Rate Pool

A company vehicle costing £35,000 in the special rate pool (6% WDA).
*)

printfn "\n=== UK Capital Allowances: Vehicle (Special Rate Pool) ==="

let companyVehicle = {
    Types.Amount = 35_000m
    Types.Pool = Types.Pool.SpecialRate
    Types.Description = "Company vehicle"
}

let vehicleConfig = {
    Types.Default with
        AnnualInvestmentAllowanceLimit = 25_000m
        MaxYears = 8
}

printfn "Cost: £%M in special rate pool (6%% WDA)" companyVehicle.Amount
printfn "AIA limit: £%M" vehicleConfig.AnnualInvestmentAllowanceLimit
printfn ""

let vehicleSchedule = Calculations.generateSchedule vehicleConfig companyVehicle

printfn "Year\tAIA\t\tWDA\t\tTotal\t\tPool EOY"
vehicleSchedule 
|> List.iter (fun year ->
    printfn "%d\t£%M\t£%M\t£%M\t£%M" 
        year.Year 
        year.AnnualInvestmentAllowance 
        year.WritingDownAllowance 
        year.TotalAllowances 
        year.PoolValueEndOfYear)

(**
## US MACRS Examples

### Example 1: Computer Equipment (5-Year Property)

Office computers costing $15,000, classified as 5-year property.
*)

printfn "\n=== US MACRS: Computer Equipment (5-Year) ==="

let computers = {
    US_MACRS.Types.OriginalBasis = 15_000m
    US_MACRS.Types.AssetClass = US_MACRS.Types.AssetClass.FiveYear
    US_MACRS.Types.Convention = "Half-Year"
}

printfn "Cost: $%M (5-year property)" computers.OriginalBasis
printfn ""

let computerSchedule = US_MACRS.Calculations.generateSchedule computers

printfn "Year\tRate\t\tDepreciation\tAccumulated\tBook Value"
computerSchedule 
|> List.iter (fun year ->
    printfn "%d\t%.2f%%\t\t$%M\t\t$%M\t\t$%M" 
        year.Year 
        (year.DepreciationRate * 100m)
        year.DepreciationAmount 
        year.AccumulatedDepreciation 
        year.BookValue)

(**
### Example 2: Office Furniture (7-Year Property)

Office furniture costing $8,000, classified as 7-year property.
*)

printfn "\n=== US MACRS: Office Furniture (7-Year) ==="

let furniture = {
    US_MACRS.Types.OriginalBasis = 8_000m
    US_MACRS.Types.AssetClass = US_MACRS.Types.AssetClass.SevenYear
    US_MACRS.Types.Convention = "Half-Year"
}

printfn "Cost: $%M (7-year property)" furniture.OriginalBasis
printfn ""

let furnitureSchedule = US_MACRS.Calculations.generateSchedule furniture

printfn "Year\tRate\t\tDepreciation\tAccumulated\tBook Value"
furnitureSchedule 
|> List.iter (fun year ->
    printfn "%d\t%.2f%%\t\t$%M\t\t$%M\t\t$%M" 
        year.Year 
        (year.DepreciationRate * 100m)
        year.DepreciationAmount 
        year.AccumulatedDepreciation 
        year.BookValue)

(**
### Example 3: Manufacturing Tools (3-Year Property)

Specialized manufacturing tools costing $5,000, classified as 3-year property.
*)

printfn "\n=== US MACRS: Manufacturing Tools (3-Year) ==="

let tools = {
    US_MACRS.Types.OriginalBasis = 5_000m
    US_MACRS.Types.AssetClass = US_MACRS.Types.AssetClass.ThreeYear
    US_MACRS.Types.Convention = "Half-Year"
}

printfn "Cost: $%M (3-year property)" tools.OriginalBasis
printfn ""

let toolsSchedule = US_MACRS.Calculations.generateSchedule tools

printfn "Year\tRate\t\tDepreciation\tAccumulated\tBook Value"
toolsSchedule 
|> List.iter (fun year ->
    printfn "%d\t%.2f%%\t\t$%M\t\t$%M\t\t$%M" 
        year.Year 
        (year.DepreciationRate * 100m)
        year.DepreciationAmount 
        year.AccumulatedDepreciation 
        year.BookValue)

(**
## Summary

This script demonstrates the key features of both depreciation modules:

### UK Capital Allowances
- Different treatment for main pool (18%) vs special rate pool (6%)
- Annual Investment Allowance application in year 1
- Writing Down Allowances for remaining value
- Midpoint-away-from-zero rounding

### US MACRS
- Different asset classes with varying recovery periods
- Half-year convention application
- Percentage-based depreciation schedules
- Complete depreciation over the recovery period

Both modules provide educational implementations of complex tax depreciation
rules and should be validated with tax professionals for actual use.
*)

printfn "\n=== Examples completed ==="