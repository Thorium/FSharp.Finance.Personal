namespace FSharp.Finance.Personal.EquipmentFinance

open System
open FSharp.Finance.Personal
open FSharp.Finance.Personal.Calculation
open FSharp.Finance.Personal.DateDay
open FSharp.Finance.Personal.Interest
open DepreciationCommon
open Macrs

/// equipment lease calculations and analysis for equipment finance
module Lease =

    /// simple power function for financial calculations
    let private pow (base': decimal) (power: decimal) = 
        decimal (System.Math.Pow(double base', double power))

    /// type of lease for equipment financing
    [<Struct; StructuredFormatDisplay("{Html}")>]
    type LeaseType =
        /// operating lease (off-balance-sheet, lessor retains ownership)
        | OperatingLease
        /// finance lease (on-balance-sheet, lessee assumes ownership risks)
        | FinanceLease
        /// capital lease (similar to finance lease under older accounting standards)
        | CapitalLease

        /// HTML formatting to display the lease type in a readable format
        member lt.Html =
            match lt with
            | OperatingLease -> "operating lease"
            | FinanceLease -> "finance lease"
            | CapitalLease -> "capital lease"

    /// lease payment frequency
    [<Struct; StructuredFormatDisplay("{Html}")>]
    type PaymentFrequency =
        /// monthly payments
        | Monthly
        /// quarterly payments
        | Quarterly
        /// semi-annual payments
        | SemiAnnual
        /// annual payments
        | Annual

        /// HTML formatting to display the payment frequency in a readable format
        member pf.Html =
            match pf with
            | Monthly -> "monthly"
            | Quarterly -> "quarterly"
            | SemiAnnual -> "semi-annual"
            | Annual -> "annual"

        /// convert frequency to number of payments per year
        member pf.PaymentsPerYear =
            match pf with
            | Monthly -> 12
            | Quarterly -> 4
            | SemiAnnual -> 2
            | Annual -> 1

    /// terms and conditions for an equipment lease
    [<Struct>]
    type EquipmentLeaseTerms = {
        /// the equipment being leased
        EquipmentDescription: string
        /// the fair market value of the equipment
        FairMarketValue: int64<Cent>
        /// the lease term in months
        TermMonths: int
        /// the type of lease
        LeaseType: LeaseType
        /// payment frequency
        PaymentFrequency: PaymentFrequency
        /// lease payment amount
        LeasePayment: int64<Cent>
        /// any upfront payment or security deposit
        UpfrontPayment: int64<Cent>
        /// residual value at end of lease term
        ResidualValue: int64<Cent>
        /// purchase option at lease end
        PurchaseOption: int64<Cent> option
        /// implicit interest rate in the lease
        ImplicitRate: Rate
    }

    /// lease payment calculation result
    [<Struct>]
    type LeaseCalculation = {
        /// the periodic lease payment
        LeasePayment: int64<Cent>
        /// total payments over the lease term
        TotalPayments: int64<Cent>
        /// total cost of leasing vs buying
        TotalCost: int64<Cent>
        /// the annual percentage rate equivalent
        AprEquivalent: Percent
        /// present value of lease payments
        PresentValue: int64<Cent>
    }

    /// lease schedule item
    [<Struct>]
    type LeaseScheduleItem = {
        /// payment number (1-based)
        PaymentNumber: int
        /// payment due date
        PaymentDate: Date
        /// the lease payment amount
        PaymentAmount: int64<Cent>
        /// principal portion (for finance leases)
        PrincipalPortion: int64<Cent>
        /// interest portion (for finance leases)
        InterestPortion: int64<Cent>
        /// remaining lease liability (for finance leases)
        RemainingLiability: int64<Cent>
    }

    /// calculate lease payment for given terms
    let calculateLeasePayment (terms: EquipmentLeaseTerms) : int64<Cent> =
        let periodsPerYear = terms.PaymentFrequency.PaymentsPerYear
        let totalPeriods = terms.TermMonths * periodsPerYear / 12
        let periodRate = 
            match terms.ImplicitRate with
            | Rate.Zero -> 0m
            | Rate.Annual (Percent rate) -> rate / 100m / decimal periodsPerYear
            | Rate.Daily (Percent rate) -> rate / 100m * 365m / decimal periodsPerYear
        
        if periodRate = 0m then
            // No interest, simple division
            (terms.FairMarketValue - terms.ResidualValue) / int64 totalPeriods
        else
            let leasableAmount = terms.FairMarketValue - terms.UpfrontPayment
            let pvOfResidual = decimal terms.ResidualValue / pow (1m + periodRate) (decimal totalPeriods)
            let amountToFinance = decimal leasableAmount - pvOfResidual
            
            let payment = amountToFinance * periodRate / (1m - pow (1m + periodRate) (decimal -totalPeriods))
            payment * 1m<Cent> |> Cent.fromDecimalCent (RoundWith System.MidpointRounding.AwayFromZero)

    /// calculate lease payment details
    let calculateLeaseDetails (terms: EquipmentLeaseTerms) : LeaseCalculation =
        let leasePayment = 
            if terms.LeasePayment > 0L<Cent> then terms.LeasePayment 
            else calculateLeasePayment terms
        
        let periodsPerYear = terms.PaymentFrequency.PaymentsPerYear
        let totalPeriods = terms.TermMonths * periodsPerYear / 12
        let totalPayments = leasePayment * int64 totalPeriods + terms.UpfrontPayment
        
        let totalCost = 
            match terms.PurchaseOption with
            | Some purchasePrice -> totalPayments + purchasePrice
            | None -> totalPayments
        
        // Calculate present value of lease payments
        let periodRate = 
            match terms.ImplicitRate with
            | Rate.Zero -> 0m
            | Rate.Annual (Percent rate) -> rate / 100m / decimal periodsPerYear
            | Rate.Daily (Percent rate) -> rate / 100m * 365m / decimal periodsPerYear
        
        let presentValue = 
            if periodRate = 0m then
                totalPayments
            else
                let pv = [1..totalPeriods]
                         |> List.sumBy (fun period -> decimal leasePayment / pow (1m + periodRate) (decimal period))
                         |> (+) (decimal terms.UpfrontPayment)
                         |> (*) 1m<Cent>
                Cent.fromDecimalCent (RoundWith System.MidpointRounding.AwayFromZero) pv
        
        let aprEquivalent = 
            match terms.ImplicitRate with
            | Rate.Zero -> Percent 0m
            | Rate.Annual percent -> percent
            | Rate.Daily (Percent rate) -> Percent (rate * 365m)
        
        {
            LeasePayment = leasePayment
            TotalPayments = totalPayments
            TotalCost = totalCost
            AprEquivalent = aprEquivalent
            PresentValue = presentValue
        }

    /// generate lease payment schedule
    let generateLeaseSchedule (terms: EquipmentLeaseTerms) (startDate: Date) : LeaseScheduleItem array =
        let leasePayment = 
            if terms.LeasePayment > 0L<Cent> then terms.LeasePayment 
            else calculateLeasePayment terms
        
        let periodsPerYear = terms.PaymentFrequency.PaymentsPerYear
        let totalPeriods = terms.TermMonths * periodsPerYear / 12
        let periodRate = 
            match terms.ImplicitRate with
            | Rate.Zero -> 0m
            | Rate.Annual (Percent rate) -> rate / 100m / decimal periodsPerYear
            | Rate.Daily (Percent rate) -> rate / 100m * 365m / decimal periodsPerYear
        
        let monthsPerPeriod = 12 / periodsPerYear
        
        let rec generateSchedule paymentNum currentDate liability acc =
            if paymentNum > totalPeriods then
                acc |> List.rev |> Array.ofList
            else
                let interestPortion = 
                    if terms.LeaseType = OperatingLease then
                        0L<Cent> // Operating leases don't split principal/interest
                    else
                        decimal liability * periodRate * 1m<Cent>
                        |> Cent.fromDecimalCent (RoundWith System.MidpointRounding.AwayFromZero)
                
                let principalPortion = 
                    if terms.LeaseType = OperatingLease then
                        0L<Cent> // Operating leases don't split principal/interest
                    else
                        leasePayment - interestPortion
                
                let newLiability = 
                    if terms.LeaseType = OperatingLease then
                        liability // No liability reduction for operating leases
                    else
                        liability - principalPortion
                
                let paymentDate = startDate.AddMonths(paymentNum * monthsPerPeriod)
                
                let item = {
                    PaymentNumber = paymentNum
                    PaymentDate = paymentDate
                    PaymentAmount = leasePayment
                    PrincipalPortion = principalPortion
                    InterestPortion = interestPortion
                    RemainingLiability = newLiability
                }
                
                generateSchedule (paymentNum + 1) paymentDate newLiability (item :: acc)
        
        let initialLiability = 
            if terms.LeaseType = OperatingLease then
                terms.FairMarketValue // For display purposes
            else
                terms.FairMarketValue - terms.UpfrontPayment
        
        generateSchedule 1 startDate initialLiability []

    /// analyze lease vs buy decision
    type LeaseVsBuyAnalysis = {
        /// lease calculation details
        LeaseDetails: LeaseCalculation
        /// depreciation schedule if equipment were purchased
        PurchaseDepreciation: AnnualDepreciation array
        /// lease payment schedule
        LeaseSchedule: LeaseScheduleItem array
        /// net advantage to leasing (positive means leasing is better)
        NetAdvantageToLeasing: int64<Cent> option
    }

    /// perform lease vs buy analysis
    let analyzeLeaseVsBuy (terms: EquipmentLeaseTerms) (startDate: Date) : LeaseVsBuyAnalysis =
        let leaseDetails = calculateLeaseDetails terms
        let leaseSchedule = generateLeaseSchedule terms startDate
        
        // Create depreciation schedule for purchase scenario
        let macrsAsset = {
            CostBasis = terms.FairMarketValue
            PlacedInServiceDate = startDate
            PropertyClass = classifyAsset terms.EquipmentDescription
            Convention = HalfYear
        }
        
        let depreciationSchedule = calculateMacrsSchedule macrsAsset
        
        {
            LeaseDetails = leaseDetails
            PurchaseDepreciation = depreciationSchedule
            LeaseSchedule = leaseSchedule
            NetAdvantageToLeasing = None // Would implement full NPV analysis in complete version
        }