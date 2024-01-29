namespace FSharp.Finance.Personal

open CustomerPayments

/// functions for handling received payments and calculating interest and/or charges where necessary
module AppliedPayment =

     /// an actual payment made on a particular day, optionally with charges applied, with the net effect and payment status calculated
    [<Struct>]
    type AppliedPayment = {
        /// the day the payment is made, as an offset of days from the start date
        AppliedPaymentDay: int<OffsetDay>
        /// the amount of any scheduled payment due on the current day
        ScheduledPayment: int64<Cent>
        /// the amounts of any actual payments made on the current day
        ActualPayments: int64<Cent> array
        /// details of any charges incurred on the current day
        IncurredCharges: Charge array
        /// the net effect of any payments made on the current day
        NetEffect: int64<Cent>
        /// the payment status based on the payments made on the current day
        PaymentStatus: CustomerPaymentStatus voption
    }

    /// applies actual payments, adds a payment status and optionally a late payment charge if underpaid
    let applyPayments asOfDay (latePaymentGracePeriod: int<DurationDay>) (latePaymentCharge: Amount voption) actualPayments scheduledPayments =
        if Array.isEmpty scheduledPayments then [||] else
        let nonZero =
            Array.filter(fun p ->
                match p.PaymentDetails with
                | ScheduledPayment sp when p.PaymentDay > 0<OffsetDay> && sp = 0L<Cent> -> false
                | ActualPayment (ap, _) when ap = 0L<Cent> -> false
                | _ -> true
            )
        [| nonZero scheduledPayments; nonZero actualPayments |]        |> Array.concat
        |> Array.groupBy _.PaymentDay
        |> Array.map(fun (offsetDay, payments) ->
            let scheduledPayment = payments |> Array.map(fun p -> p.PaymentDetails |> function ScheduledPayment p -> p | _ -> 0L<Cent>) |> Array.sum
            let actualPayments = payments |> Array.choose(fun p -> p.PaymentDetails |> function ActualPayment (ap, _) -> Some ap | _ -> None)
            let netEffect, paymentStatus =
                match scheduledPayment, Array.sum actualPayments with
                | 0L<Cent>, 0L<Cent> -> 0L<Cent>, ValueNone
                | 0L<Cent>, ap when ap < 0L<Cent> -> ap, ValueSome Refunded
                | 0L<Cent>, ap -> ap, ValueSome ExtraPayment
                | sp, _ when (offsetDay < asOfDay) && (int offsetDay + int latePaymentGracePeriod >= int asOfDay) -> sp, ValueSome WithinGracePeriod
                | sp, _ when offsetDay >= asOfDay -> sp, ValueSome NotYetDue
                | _, 0L<Cent> -> 0L<Cent>, ValueSome MissedPayment
                | sp, ap when ap < sp -> ap, ValueSome Underpayment
                | sp, ap when ap > sp -> ap, ValueSome Overpayment
                | _, ap -> ap, ValueSome PaymentMade
            let charges =
                payments
                |> Array.collect(fun p -> p.PaymentDetails |> function ActualPayment (_, c) -> c | _ -> [||])
                |> fun pcc ->
                    if latePaymentCharge.IsSome then
                        pcc |> Array.append(match paymentStatus with ValueSome MissedPayment | ValueSome Underpayment -> [| Charge.LatePayment latePaymentCharge.Value |] | _ -> [||])
                    else pcc
            { AppliedPaymentDay = offsetDay; ScheduledPayment = scheduledPayment; ActualPayments = actualPayments; IncurredCharges = charges; NetEffect = netEffect; PaymentStatus = paymentStatus }
        )
        |> Array.sortBy _.AppliedPaymentDay