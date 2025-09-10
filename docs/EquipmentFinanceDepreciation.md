# Equipment Finance Depreciation Modules

This document describes the Equipment Finance depreciation modules included in FSharp.Finance.Personal, providing implementations for both UK Capital Allowances and US MACRS depreciation calculations.

## Overview

The Equipment Finance depreciation modules are located under the namespace `FSharp.Finance.Personal.EquipmentFinance.Depreciation` and provide:

- **UK Capital Allowances** (`UK_CapitalAllowances`): Implements UK tax depreciation calculations including Annual Investment Allowance (AIA) and Writing Down Allowances (WDA)
- **US MACRS** (`US_MACRS`): Implements US Modified Accelerated Cost Recovery System depreciation calculations

## Important Disclaimers

⚠️ **EDUCATIONAL PURPOSES ONLY**: These modules are for educational and analytical purposes only. They are NOT tax advice and should not be used for actual tax calculations without validation by qualified tax professionals.

Both modules include several simplifications that may not reflect real-world tax scenarios. Users must validate all calculations with appropriate tax authorities and professionals.

## UK Capital Allowances Module

### Namespace
```fsharp
FSharp.Finance.Personal.EquipmentFinance.Depreciation.UK_CapitalAllowances
```

### Key Features

- **Pool Classifications**: Main pool (18% WDA) and Special rate pool (6% WDA)
- **Annual Investment Allowance**: Configurable AIA limit (default £1,000,000)
- **Writing Down Allowances**: Automatic calculation based on pool type
- **Rounding**: Uses midpoint-away-from-zero rounding for precision

### Basic Usage

```fsharp
open FSharp.Finance.Personal.EquipmentFinance.Depreciation.UK_CapitalAllowances

// Define an expenditure
let machinery = {
    Amount = 50_000m
    Pool = Types.Pool.Main
    Description = "Manufacturing equipment"
}

// Generate depreciation schedule using default configuration
let schedule = Calculations.scheduleDefault machinery

// Print results
schedule 
|> List.iter (fun year ->
    printfn "Year %d: AIA £%.2f, WDA £%.2f, Total £%.2f" 
        year.Year 
        year.AnnualInvestmentAllowance 
        year.WritingDownAllowance 
        year.TotalAllowances)
```

### Key Simplifications

- Single asset addition per year only
- No disposals or part-exchanges
- No special rate pool transfers
- Simplified AIA application (full amount in year 1 only)
- No consideration of accounting periods vs tax years
- No integration with other allowances or reliefs

## US MACRS Module

### Namespace
```fsharp
FSharp.Finance.Personal.EquipmentFinance.Depreciation.US_MACRS
```

### Key Features

- **Asset Classifications**: 3, 5, 7, 10, 15, and 20-year property classes
- **Half-Year Convention**: Simplified half-year convention tables
- **Depreciation Schedules**: Complete year-by-year depreciation calculations
- **Educational Tables**: Based on common IRS depreciation percentages

### Basic Usage

```fsharp
open FSharp.Finance.Personal.EquipmentFinance.Depreciation.US_MACRS

// Define an asset
let computer = {
    OriginalBasis = 10_000m
    AssetClass = Types.AssetClass.FiveYear
    Convention = "Half-Year"
}

// Generate depreciation schedule
let schedule = Calculations.generateSchedule computer

// Print results
schedule 
|> List.iter (fun year ->
    printfn "Year %d: Rate %.2f%%, Depreciation $%.2f, Book Value $%.2f" 
        year.Year 
        (year.DepreciationRate * 100m)
        year.DepreciationAmount 
        year.BookValue)
```

### Asset Classes

| Class | Recovery Period | Typical Assets |
|-------|----------------|----------------|
| 3-Year | 3 years | Special tools, small manufacturing equipment |
| 5-Year | 5 years | Computers, office machinery, vehicles |
| 7-Year | 7 years | Office furniture, most equipment |
| 10-Year | 10 years | Boats, barges, single-purpose structures |
| 15-Year | 15 years | Land improvements, gas stations |
| 20-Year | 20 years | Farm buildings, utility property |

### Key Simplifications

- Limited to half-year convention only
- Simplified asset class definitions
- No mid-quarter convention handling
- No Section 179 deduction integration
- No bonus depreciation considerations
- Educational percentage tables only

## Examples

### UK Example: Vehicle in Special Rate Pool

```fsharp
let vehicle = {
    Amount = 30_000m
    Pool = Types.Pool.SpecialRate
    Description = "Company vehicle"
}

let customConfig = {
    Types.Default with
        AnnualInvestmentAllowanceLimit = 25_000m
        MaxYears = 5
}

let schedule = Calculations.generateSchedule customConfig vehicle
```

### US Example: Manufacturing Equipment

```fsharp
let equipment = {
    OriginalBasis = 25_000m
    AssetClass = Types.AssetClass.SevenYear
    Convention = "Half-Year"
}

let schedule = Calculations.generateSchedule equipment
```

## Testing

Both modules include comprehensive unit tests that verify:

- Correct application of depreciation rules
- Accurate mathematical calculations
- Proper handling of edge cases
- Compliance with expected tax principles

Tests are located in:
- `tests/UKCapitalAllowancesTests.fs`
- `tests/USMacrsTests.fs`

## Integration

These modules integrate seamlessly with the existing FSharp.Finance.Personal library and can be used alongside other financial calculations for comprehensive equipment finance analysis.

## Future Enhancements

Potential future enhancements could include:

- Additional depreciation methods
- More comprehensive asset classifications
- Integration with other tax calculations
- Support for partial-year conventions
- Disposal and replacement scenarios

---

**Note**: This supersedes PR #4 by delivering both US and UK equipment finance depreciation components in a consistent structure with proper namespacing, supporting tests, and comprehensive documentation.