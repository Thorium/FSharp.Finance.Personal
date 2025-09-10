namespace FSharp.Finance.Personal.EquipmentFinance.Depreciation

/// Common depreciation abstractions and utilities shared across depreciation modules.
/// 
/// This module provides common types, interfaces, and utility functions that can be
/// used across different depreciation calculation methods (UK Capital Allowances,
/// US MACRS, etc.).

module Common =
    
    /// Represents a generic depreciation period
    type DepreciationPeriod = {
        /// Period number (1-based)
        Period: int
        /// Depreciation amount for this period
        Amount: decimal
        /// Cumulative depreciation to date
        CumulativeAmount: decimal
        /// Remaining value after this period
        RemainingValue: decimal
    }

    /// Represents basic asset information
    type AssetInfo = {
        /// Original cost or basis of the asset
        OriginalCost: decimal
        /// Asset description
        Description: string
        /// Date asset was placed in service (optional)
        PlacedInServiceDate: System.DateTime option
    }

    /// Common rounding utilities for depreciation calculations
    module Rounding =
        
        /// Rounds a decimal value to 2 decimal places using midpoint-away-from-zero
        let roundCurrency (value: decimal) =
            System.Math.Round(value, 2, System.MidpointRounding.AwayFromZero)
        
        /// Rounds a decimal value to specified decimal places using midpoint-away-from-zero
        let roundToPlaces (places: int) (value: decimal) =
            System.Math.Round(value, places, System.MidpointRounding.AwayFromZero)
        
        /// Rounds a percentage to 4 decimal places
        let roundPercentage (value: decimal) =
            System.Math.Round(value, 4, System.MidpointRounding.AwayFromZero)

    /// Common validation utilities
    module Validation =
        
        /// Validates that an amount is positive
        let validatePositiveAmount (amount: decimal) (fieldName: string) =
            if amount <= 0m then
                failwith $"{fieldName} must be positive, got {amount}"
        
        /// Validates that a percentage is between 0 and 1
        let validatePercentage (percentage: decimal) (fieldName: string) =
            if percentage < 0m || percentage > 1m then
                failwith $"{fieldName} must be between 0 and 1, got {percentage}"
        
        /// Validates that a year count is positive
        let validateYearCount (years: int) (fieldName: string) =
            if years <= 0 then
                failwith $"{fieldName} must be positive, got {years}"

    /// Common calculation utilities
    module Calculations =
        
        /// Calculates the remaining value after depreciation
        let calculateRemainingValue (originalCost: decimal) (cumulativeDepreciation: decimal) =
            Rounding.roundCurrency (originalCost - cumulativeDepreciation)
        
        /// Applies a percentage rate to a base amount
        let applyRate (baseAmount: decimal) (rate: decimal) =
            Rounding.roundCurrency (baseAmount * rate)
        
        /// Calculates cumulative depreciation up to a given period
        let calculateCumulative (depreciationAmounts: decimal list) =
            depreciationAmounts |> List.scan (+) 0m |> List.tail
        
        /// Ensures total depreciation does not exceed original cost
        let capDepreciationAtCost (originalCost: decimal) (proposedDepreciation: decimal) (cumulativeDepreciation: decimal) =
            let maxAllowable = originalCost - cumulativeDepreciation
            min proposedDepreciation maxAllowable

    /// Common formatting utilities
    module Formatting =
        
        /// Formats a currency amount with appropriate symbol
        let formatCurrency (symbol: string) (amount: decimal) =
            $"{symbol}{amount:N2}"
        
        /// Formats a percentage with specified decimal places
        let formatPercentage (decimalPlaces: int) (percentage: decimal) =
            $"{percentage * 100m:N{decimalPlaces}}%%"
        
        /// Creates a formatted summary line for a depreciation period
        let formatPeriodSummary (period: DepreciationPeriod) (currencySymbol: string) =
            $"Period {period.Period}: {formatCurrency currencySymbol period.Amount} (Cumulative: {formatCurrency currencySymbol period.CumulativeAmount}, Remaining: {formatCurrency currencySymbol period.RemainingValue})"

    /// Common interfaces for depreciation methods
    module Interfaces =
        
        /// Interface for depreciation calculation methods
        type IDepreciationMethod<'TConfig, 'TAsset, 'TResult> =
            abstract member Calculate: config:'TConfig -> asset:'TAsset -> 'TResult list
            abstract member Validate: config:'TConfig -> asset:'TAsset -> unit
            abstract member GetMethodName: unit -> string
        
        /// Interface for depreciation schedule formatting
        type IDepreciationFormatter<'TResult> =
            abstract member FormatSchedule: schedule:'TResult list -> string
            abstract member FormatSummary: schedule:'TResult list -> string

    /// Disclaimer text for educational use
    module Disclaimers =
        
        /// Standard educational disclaimer for all depreciation modules
        let EducationalDisclaimer = 
            "IMPORTANT DISCLAIMER: This module is for educational and analytical purposes only. " +
            "It is NOT tax advice and should not be used for actual tax calculations without " +
            "validation by qualified tax professionals. The implementation includes several " +
            "simplifications that may not reflect real-world tax scenarios."
        
        /// UK-specific disclaimer additions
        let UKSpecificDisclaimer =
            "This implementation is based on general UK capital allowances rules and may not " +
            "reflect recent changes, special circumstances, or specific industry rules. " +
            "Always consult HMRC guidance and qualified tax advisors."
        
        /// US-specific disclaimer additions
        let USSpecificDisclaimer =
            "This implementation is based on general US MACRS rules and may not reflect " +
            "recent tax law changes, special circumstances, or state-specific rules. " +
            "Always consult IRS publications and qualified tax professionals."