namespace FSharp.Finance.Personal.EquipmentFinance

open System
open FSharp.Finance.Personal
open FSharp.Finance.Personal.Calculation
open FSharp.Finance.Personal.DateDay
open FSharp.Finance.Personal.Interest
open DepreciationCommon
open Macrs

/// equipment loan calculations and analysis for equipment finance
module Loan =

    /// simple power function for financial calculations
    let private pow (base': decimal) (power: decimal) = 
        decimal (System.Math.Pow(double base', double power))

    /// terms and conditions for an equipment loan
    [<Struct>]
    type EquipmentLoanTerms = {
        /// the principal amount of the loan
        Principal: int64<Cent>
        /// the annual interest rate
        InterestRate: Rate
        /// the loan term in months
        TermMonths: int
        /// the monthly payment amount (if known)
        MonthlyPayment: int64<Cent> option
        /// the equipment being financed
        EquipmentDescription: string
        /// the cost of the equipment
        EquipmentCost: int64<Cent>
        /// down payment made on the equipment
        DownPayment: int64<Cent>
        /// residual value at end of loan term
        ResidualValue: int64<Cent>
    }

    /// loan payment calculation result
    [<Struct>]
    type PaymentCalculation = {
        /// the calculated monthly payment
        MonthlyPayment: int64<Cent>
        /// total payments over the loan term
        TotalPayments: int64<Cent>
        /// total interest paid over the loan term
        TotalInterest: int64<Cent>
        /// the annual percentage rate
        Apr: Percent
    }

    /// loan amortization schedule item
    [<Struct>]
    type AmortizationItem = {
        /// payment number (1-based)
        PaymentNumber: int
        /// payment due date
        PaymentDate: Date
        /// the payment amount
        PaymentAmount: int64<Cent>
        /// principal portion of payment
        PrincipalPayment: int64<Cent>
        /// interest portion of payment
        InterestPayment: int64<Cent>
        /// remaining principal balance
        RemainingBalance: int64<Cent>
    }

    /// calculate monthly payment for an equipment loan
    let calculateMonthlyPayment (terms: EquipmentLoanTerms) : int64<Cent> =
        match terms.MonthlyPayment with
        | Some payment -> payment
        | None ->
            let monthlyRate = 
                match terms.InterestRate with
                | Rate.Zero -> 0m
                | Rate.Annual (Percent rate) -> rate / 100m / 12m
                | Rate.Daily (Percent rate) -> rate / 100m * 365m / 12m
            
            if monthlyRate = 0m then
                // No interest, just divide principal by term
                terms.Principal / int64 terms.TermMonths
            else
                let loanAmount = terms.Principal - terms.ResidualValue
                let numerator = decimal loanAmount * monthlyRate * pow (1m + monthlyRate) (decimal terms.TermMonths)
                let denominator = pow (1m + monthlyRate) (decimal terms.TermMonths) - 1m
                let payment = numerator / denominator
                payment * 1m<Cent> |> Cent.fromDecimalCent (RoundWith MidpointRounding.AwayFromZero)

    /// calculate payment details for an equipment loan
    let calculatePaymentDetails (terms: EquipmentLoanTerms) : PaymentCalculation =
        let monthlyPayment = calculateMonthlyPayment terms
        let totalPayments = monthlyPayment * int64 terms.TermMonths + terms.ResidualValue
        let totalInterest = totalPayments - terms.Principal
        
        // Simplified APR calculation (actual APR would require iterative calculation)
        let annualRate = 
            match terms.InterestRate with
            | Rate.Zero -> Percent 0m
            | Rate.Annual percent -> percent
            | Rate.Daily (Percent rate) -> Percent (rate * 365m)
        
        {
            MonthlyPayment = monthlyPayment
            TotalPayments = totalPayments
            TotalInterest = totalInterest
            Apr = annualRate
        }

    /// generate loan amortization schedule
    let generateAmortizationSchedule (terms: EquipmentLoanTerms) (startDate: Date) : AmortizationItem array =
        let monthlyPayment = calculateMonthlyPayment terms
        let monthlyRate = 
            match terms.InterestRate with
            | Rate.Zero -> 0m
            | Rate.Annual (Percent rate) -> rate / 100m / 12m
            | Rate.Daily (Percent rate) -> rate / 100m * 365m / 12m
        
        let rec generateSchedule paymentNum currentDate balance acc =
            if paymentNum > terms.TermMonths then
                acc |> List.rev |> Array.ofList
            else
                let interestPayment = 
                    decimal balance * monthlyRate * 1m<Cent>
                    |> Cent.fromDecimalCent (RoundWith System.MidpointRounding.AwayFromZero)
                
                let principalPayment = 
                    if paymentNum = terms.TermMonths then
                        // Final payment: pay remaining balance minus residual
                        balance - terms.ResidualValue
                    else
                        monthlyPayment - interestPayment
                
                let newBalance = balance - principalPayment
                let paymentDate = startDate.AddMonths(paymentNum)
                
                let item = {
                    PaymentNumber = paymentNum
                    PaymentDate = paymentDate
                    PaymentAmount = if paymentNum = terms.TermMonths then principalPayment + interestPayment else monthlyPayment
                    PrincipalPayment = principalPayment
                    InterestPayment = interestPayment
                    RemainingBalance = newBalance
                }
                
                generateSchedule (paymentNum + 1) paymentDate newBalance (item :: acc)
        
        generateSchedule 1 startDate terms.Principal []

    /// analyze equipment loan with depreciation considerations
    type LoanAnalysis = {
        /// loan payment calculation
        PaymentDetails: PaymentCalculation
        /// depreciation schedule for the equipment
        DepreciationSchedule: AnnualDepreciation array
        /// loan amortization schedule
        AmortizationSchedule: AmortizationItem array
        /// net present value analysis would go here in a full implementation
        NetPresentValue: int64<Cent> option
    }

    /// perform comprehensive analysis of an equipment loan
    let analyzeLoan (terms: EquipmentLoanTerms) (startDate: Date) : LoanAnalysis =
        let paymentDetails = calculatePaymentDetails terms
        let amortizationSchedule = generateAmortizationSchedule terms startDate
        
        // Create MACRS asset for depreciation analysis
        let macrsAsset = {
            CostBasis = terms.EquipmentCost
            PlacedInServiceDate = startDate
            PropertyClass = classifyAsset terms.EquipmentDescription
            Convention = HalfYear
        }
        
        let depreciationSchedule = calculateMacrsSchedule macrsAsset
        
        {
            PaymentDetails = paymentDetails
            DepreciationSchedule = depreciationSchedule
            AmortizationSchedule = amortizationSchedule
            NetPresentValue = None // Would implement NPV calculation in full version
        }