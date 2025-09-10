namespace FSharp.Finance.Personal.EquipmentFinance

open System
open FSharp.Finance.Personal
open FSharp.Finance.Personal.Calculation
open FSharp.Finance.Personal.DateDay
open DepreciationCommon

/// Modified Accelerated Cost Recovery System (MACRS) depreciation calculations for equipment finance
module Macrs =

    /// MACRS property class based on asset recovery period
    [<Struct; StructuredFormatDisplay("{Html}")>]
    type PropertyClass =
        /// 3-year property (e.g., tractors, racehorses over 2 years old)
        | ThreeYear
        /// 5-year property (e.g., cars, trucks, computers, office equipment)
        | FiveYear
        /// 7-year property (e.g., office furniture, equipment)
        | SevenYear
        /// 10-year property (e.g., water transportation equipment)
        | TenYear
        /// 15-year property (e.g., land improvements, restaurants)
        | FifteenYear
        /// 20-year property (e.g., farm buildings)
        | TwentyYear

        /// HTML formatting to display the property class in a readable format
        member pc.Html =
            match pc with
            | ThreeYear -> "3-year property"
            | FiveYear -> "5-year property"
            | SevenYear -> "7-year property"
            | TenYear -> "10-year property"
            | FifteenYear -> "15-year property"
            | TwentyYear -> "20-year property"

    /// MACRS depreciation convention
    [<Struct; StructuredFormatDisplay("{Html}")>]
    type Convention =
        /// half-year convention (most common)
        | HalfYear
        /// mid-quarter convention (when over 40% of assets placed in service in Q4)
        | MidQuarter

        /// HTML formatting to display the convention in a readable format
        member c.Html =
            match c with
            | HalfYear -> "half-year"
            | MidQuarter -> "mid-quarter"

    /// MACRS percentage tables for half-year convention
    let private macrsHalfYearPercentages = [|
        // 3-year
        [| 33.33m; 44.45m; 14.81m; 7.41m |]
        // 5-year  
        [| 20.00m; 32.00m; 19.20m; 11.52m; 11.52m; 5.76m |]
        // 7-year
        [| 14.29m; 24.49m; 17.49m; 12.49m; 8.93m; 8.92m; 8.93m; 4.46m |]
        // 10-year
        [| 10.00m; 18.00m; 14.40m; 11.52m; 9.22m; 7.37m; 6.55m; 6.55m; 6.56m; 6.55m; 3.28m |]
        // 15-year
        [| 5.00m; 9.50m; 8.55m; 7.70m; 6.93m; 6.23m; 5.90m; 5.90m; 5.91m; 5.90m; 5.91m; 5.90m; 5.91m; 5.90m; 5.91m; 2.95m |]
        // 20-year
        [| 3.75m; 7.22m; 6.68m; 6.18m; 5.71m; 5.29m; 4.89m; 4.52m; 4.46m; 4.46m; 4.46m; 4.46m; 4.46m; 4.46m; 4.46m; 4.46m; 4.46m; 4.46m; 4.46m; 4.46m; 2.25m |]
    |]

    /// get the MACRS percentage for a given property class and year
    let private getMacrsPercentage (propertyClass: PropertyClass) (year: int) : decimal =
        let tableIndex = 
            match propertyClass with
            | ThreeYear -> 0
            | FiveYear -> 1
            | SevenYear -> 2
            | TenYear -> 3
            | FifteenYear -> 4
            | TwentyYear -> 5
        
        let percentages = macrsHalfYearPercentages.[tableIndex]
        if year > 0 && year <= percentages.Length then
            percentages.[year - 1]
        else
            0m

    /// MACRS asset information
    [<Struct>]
    type MacrsAsset = {
        /// the original cost basis of the asset
        CostBasis: int64<Cent>
        /// the date the asset was placed in service
        PlacedInServiceDate: Date
        /// the MACRS property class
        PropertyClass: PropertyClass
        /// the depreciation convention to apply
        Convention: Convention
    }

    /// calculate MACRS depreciation for a specific year
    let calculateMacrsDepreciation (asset: MacrsAsset) (year: int) : AnnualDepreciation =
        let percentage = getMacrsPercentage asset.PropertyClass year
        let depreciationAmount = 
            decimal asset.CostBasis * (percentage / 100m) * 1m<Cent>
            |> Cent.fromDecimalCent (RoundWith MidpointRounding.AwayFromZero)
        
        // Calculate cumulative depreciation through this year
        let cumulativeDepreciation = 
            [1..year]
            |> List.sumBy (fun y -> 
                let pct = getMacrsPercentage asset.PropertyClass y
                decimal asset.CostBasis * (pct / 100m))
            |> (*) 1m<Cent>
            |> Cent.fromDecimalCent (RoundWith MidpointRounding.AwayFromZero)
            |> min asset.CostBasis
        
        let bookValue = asset.CostBasis - cumulativeDepreciation
        
        {
            Year = year
            DepreciationAmount = depreciationAmount
            CumulativeDepreciation = cumulativeDepreciation
            BookValue = bookValue
        }

    /// calculate complete MACRS depreciation schedule for an asset
    let calculateMacrsSchedule (asset: MacrsAsset) : AnnualDepreciation array =
        let maxYears = 
            match asset.PropertyClass with
            | ThreeYear -> 4  // includes half-year in final year
            | FiveYear -> 6
            | SevenYear -> 8
            | TenYear -> 11
            | FifteenYear -> 16
            | TwentyYear -> 21
        
        [| 1..maxYears |]
        |> Array.map (calculateMacrsDepreciation asset)

    /// determine the MACRS property class based on asset description
    let classifyAsset (assetDescription: string) : PropertyClass =
        let desc = assetDescription.ToLowerInvariant()
        if desc.Contains("computer") || desc.Contains("car") || desc.Contains("truck") then
            FiveYear
        elif desc.Contains("furniture") || desc.Contains("equipment") then
            SevenYear
        elif desc.Contains("building") then
            FifteenYear
        else
            FiveYear // default to 5-year for most business equipment