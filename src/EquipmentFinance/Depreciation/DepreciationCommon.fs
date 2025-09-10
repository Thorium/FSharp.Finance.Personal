namespace FSharp.Finance.Personal.EquipmentFinance

open System
open FSharp.Finance.Personal
open FSharp.Finance.Personal.Calculation
open FSharp.Finance.Personal.DateDay

/// common depreciation types and functions for equipment finance calculations
module DepreciationCommon =

    /// the type of asset for depreciation purposes
    [<Struct; StructuredFormatDisplay("{Html}")>]
    type AssetType =
        /// tangible personal property used in business
        | PersonalProperty
        /// real estate and buildings
        | RealProperty
        /// intangible assets like patents and copyrights
        | IntangibleProperty

        /// HTML formatting to display the asset type in a readable format
        member at.Html =
            match at with
            | PersonalProperty -> "personal property"
            | RealProperty -> "real property"
            | IntangibleProperty -> "intangible property"

    /// depreciation method to be applied
    [<Struct; StructuredFormatDisplay("{Html}")>]
    type DepreciationMethod =
        /// Modified Accelerated Cost Recovery System
        | MACRS
        /// straight-line depreciation over asset life
        | StraightLine of Years: int
        /// declining balance method
        | DecliningBalance of Rate: Percent

        /// HTML formatting to display the depreciation method in a readable format
        member dm.Html =
            match dm with
            | MACRS -> "MACRS"
            | StraightLine years -> $"straight-line over {years} years"
            | DecliningBalance (Percent rate) -> $"declining balance at {rate:F2}%%"

    /// basic asset information for depreciation calculations
    [<Struct>]
    type AssetInfo = {
        /// the original cost basis of the asset
        CostBasis: int64<Cent>
        /// the date the asset was placed in service
        PlacedInServiceDate: Date
        /// the type of asset for depreciation classification
        AssetType: AssetType
        /// the depreciation method to apply
        DepreciationMethod: DepreciationMethod
    }

    /// annual depreciation calculation result
    [<Struct>]
    type AnnualDepreciation = {
        /// the tax year for this depreciation
        Year: int
        /// the depreciation amount for this year
        DepreciationAmount: int64<Cent>
        /// the cumulative depreciation to date
        CumulativeDepreciation: int64<Cent>
        /// the remaining book value after this year's depreciation
        BookValue: int64<Cent>
    }

    /// calculate straight-line depreciation for a given year
    let calculateStraightLine (assetInfo: AssetInfo) (year: int) : AnnualDepreciation =
        match assetInfo.DepreciationMethod with
        | StraightLine years ->
            let annualDepreciation = assetInfo.CostBasis / int64 years
            let cumulativeDepreciation = int64 year * annualDepreciation |> min assetInfo.CostBasis
            let bookValue = assetInfo.CostBasis - cumulativeDepreciation
            {
                Year = year
                DepreciationAmount = if year <= years then annualDepreciation else 0L<Cent>
                CumulativeDepreciation = cumulativeDepreciation
                BookValue = bookValue
            }
        | _ -> failwith "Invalid depreciation method for straight-line calculation"

    /// calculate declining balance depreciation for a given year
    let calculateDecliningBalance (assetInfo: AssetInfo) (year: int) : AnnualDepreciation =
        match assetInfo.DepreciationMethod with
        | DecliningBalance rate ->
            let depreciationRateDecimal = Percent.toDecimal rate
            
            let rec calculateYear currentYear bookValue cumulative =
                if currentYear > year then
                    {
                        Year = year
                        DepreciationAmount = 0L<Cent>
                        CumulativeDepreciation = cumulative
                        BookValue = bookValue
                    }
                else
                    let yearlyDepreciation = decimal bookValue * depreciationRateDecimal * 1m<Cent> |> Cent.fromDecimalCent (RoundWith MidpointRounding.AwayFromZero)
                    let newBookValue = bookValue - yearlyDepreciation
                    let newCumulative = cumulative + yearlyDepreciation
                    
                    if currentYear = year then
                        {
                            Year = year
                            DepreciationAmount = yearlyDepreciation
                            CumulativeDepreciation = newCumulative
                            BookValue = newBookValue
                        }
                    else
                        calculateYear (currentYear + 1) newBookValue newCumulative
            
            calculateYear 1 assetInfo.CostBasis 0L<Cent>
        | _ -> failwith "Invalid depreciation method for declining balance calculation"