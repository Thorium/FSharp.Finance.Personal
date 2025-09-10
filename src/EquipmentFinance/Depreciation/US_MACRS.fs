namespace FSharp.Finance.Personal.EquipmentFinance.Depreciation.US_MACRS

/// US MACRS (Modified Accelerated Cost Recovery System) module for equipment depreciation calculations.
/// 
/// IMPORTANT DISCLAIMER: This module is for educational and analytical purposes only.
/// It is NOT tax advice and should not be used for actual tax calculations without
/// validation by qualified tax professionals. The implementation includes several
/// simplifications that may not reflect real-world tax scenarios.
///
/// Key simplifications:
/// - Limited to half-year convention only
/// - Simplified asset class definitions
/// - No mid-quarter convention handling
/// - No Section 179 deduction integration
/// - No bonus depreciation considerations
/// - No averaging conventions for real property
/// - Simplified recovery period options
/// - No consideration of placed-in-service dates
/// - Educational percentage tables only

module Types =
    
    /// Represents MACRS asset classes with their typical recovery periods
    [<RequireQualifiedAccess>]
    type AssetClass =
        | ThreeYear     // Certain special tools, small manufacturing equipment
        | FiveYear      // Most equipment, computers, office machinery, vehicles
        | SevenYear     // Office furniture, equipment not in other classes
        | TenYear       // Boats, barges, single-purpose structures
        | FifteenYear   // Land improvements, gas stations, billboards
        | TwentyYear    // Farm buildings, utility property

    /// Recovery period in years
    type RecoveryPeriod = int

    /// Represents a year in the depreciation schedule
    type DepreciationYear = {
        /// Year number (1-based)
        Year: int
        /// Depreciation percentage for this year
        DepreciationRate: decimal
        /// Annual depreciation amount
        DepreciationAmount: decimal
        /// Accumulated depreciation to date
        AccumulatedDepreciation: decimal
        /// Remaining book value
        BookValue: decimal
    }

    /// Configuration for MACRS calculations
    type MacrsConfig = {
        /// Original cost/basis of the asset
        OriginalBasis: decimal
        /// Asset classification
        AssetClass: AssetClass
        /// Convention used (currently only half-year supported)
        Convention: string
    }

module Tables =
    
    open Types

    /// Half-year convention depreciation percentages by asset class and year
    /// These are simplified educational tables - actual IRS tables should be used for real calculations
    let HalfYearPercentages = 
        Map.ofList [
            (AssetClass.ThreeYear, [| 33.33m; 44.45m; 14.81m; 7.41m |])
            (AssetClass.FiveYear, [| 20.00m; 32.00m; 19.20m; 11.52m; 11.52m; 5.76m |])
            (AssetClass.SevenYear, [| 14.29m; 24.49m; 17.49m; 12.49m; 8.93m; 8.92m; 8.93m; 4.46m |])
            (AssetClass.TenYear, [| 10.00m; 18.00m; 14.40m; 11.52m; 9.22m; 7.37m; 6.55m; 6.55m; 6.56m; 6.55m; 3.28m |])
            (AssetClass.FifteenYear, [| 5.00m; 9.50m; 8.55m; 7.70m; 6.93m; 6.23m; 5.90m; 5.90m; 5.91m; 5.90m; 5.91m; 5.90m; 5.91m; 5.90m; 5.91m; 2.95m |])
            (AssetClass.TwentyYear, [| 3.750m; 7.219m; 6.677m; 6.177m; 5.713m; 5.285m; 4.888m; 4.522m; 4.462m; 4.461m; 4.462m; 4.461m; 4.462m; 4.461m; 4.462m; 4.461m; 4.462m; 4.461m; 4.462m; 4.461m; 2.231m |])
        ]

    /// Get the depreciation percentages for a given asset class
    let getDepreciationPercentages (assetClass: AssetClass) : decimal array =
        match HalfYearPercentages.TryFind assetClass with
        | Some percentages -> percentages
        | None -> [| 0m |] // Fallback for unsupported classes

module Calculations =
    
    open Types
    open Tables

    /// Calculate the MACRS depreciation schedule for an asset
    let generateSchedule (config: MacrsConfig) : DepreciationYear list =
        let percentages = getDepreciationPercentages config.AssetClass
        
        let rec calculateYears (year: int) (accumulatedDep: decimal) (acc: DepreciationYear list) =
            if year > percentages.Length then
                List.rev acc
            else
                let rate = percentages.[year - 1] / 100m // Convert percentage to decimal
                let depreciationAmount = config.OriginalBasis * rate
                let newAccumulated = accumulatedDep + depreciationAmount
                let bookValue = config.OriginalBasis - newAccumulated

                let depreciationYear = {
                    Year = year
                    DepreciationRate = rate
                    DepreciationAmount = System.Math.Round(depreciationAmount, 2)
                    AccumulatedDepreciation = System.Math.Round(newAccumulated, 2)
                    BookValue = System.Math.Round(bookValue, 2)
                }

                calculateYears (year + 1) newAccumulated (depreciationYear :: acc)

        calculateYears 1 0m []

    /// Get the recovery period for an asset class
    let getRecoveryPeriod (assetClass: AssetClass) : RecoveryPeriod =
        match assetClass with
        | AssetClass.ThreeYear -> 3
        | AssetClass.FiveYear -> 5
        | AssetClass.SevenYear -> 7
        | AssetClass.TenYear -> 10
        | AssetClass.FifteenYear -> 15
        | AssetClass.TwentyYear -> 20

module Examples =
    
    open Types
    open Calculations

    /// Example: Computer equipment costing $10,000 (5-year property)
    let exampleComputer = {
        OriginalBasis = 10_000m
        AssetClass = AssetClass.FiveYear
        Convention = "Half-Year"
    }

    /// Example: Office furniture costing $5,000 (7-year property)
    let exampleFurniture = {
        OriginalBasis = 5_000m
        AssetClass = AssetClass.SevenYear
        Convention = "Half-Year"
    }

    /// Example: Manufacturing equipment costing $25,000 (7-year property)
    let exampleEquipment = {
        OriginalBasis = 25_000m
        AssetClass = AssetClass.SevenYear
        Convention = "Half-Year"
    }

    /// Generate example schedule for computer
    let exampleComputerSchedule () = generateSchedule exampleComputer

    /// Generate example schedule for furniture
    let exampleFurnitureSchedule () = generateSchedule exampleFurniture

    /// Generate example schedule for equipment
    let exampleEquipmentSchedule () = generateSchedule exampleEquipment