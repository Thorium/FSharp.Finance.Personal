namespace FSharp.Finance.Personal.Tests

open System
open Xunit
open FsUnit.Xunit

open FSharp.Finance.Personal

module AprUnitedKingdomTests =

    open Apr
    open UnitPeriod

    [<Fact>]
    let ``Quirky-old. APR calculation 1 payment zero`` () =
        UnitedKingdom.``APR Calculation`` (DateTime(2012,10,10)) (Cent.toDecimal 50000L<Cent>) [| (DateTime(2012, 10, 10)), (Cent.toDecimal 50000L<Cent>) |]
        |> should (equalWithin 0.001) 0.0m

    [<Fact>] 
    let``Quirky-old. APR calculation 1 payment`` () =
        UnitedKingdom.``APR Calculation`` (DateTime(2012,10,10)) (Cent.toDecimal 50000L<Cent>) [| (DateTime(2012, 10, 15)), (Cent.toDecimal 51000L<Cent>) |]
        |> should (equalWithin 0.001) 324.436m

    [<Fact>] 
    let``Quirky-old. APR calculation 2 payments`` () =
        UnitedKingdom.``APR Calculation`` (DateTime(2012,10,10)) (Cent.toDecimal 50000L<Cent>) [| (DateTime(2012, 11, 10)), (Cent.toDecimal 27000L<Cent>); (DateTime(2012, 12, 10)), (Cent.toDecimal 27000L<Cent>) |]
        |> should (equalWithin 0.001) 84.63m

    [<Fact>]
    let ``Quirky. APR calculation 1 payment zero`` () =
        UnitedKingdom.calculateApr (DateTime(2012,10,10)) 50000L<Cent> [| { TransferType = Payment; Date = DateTime(2012, 10, 10); Amount = 50000L<Cent> } |]
        |> ValueOption.defaultValue (Percent 0m)
        |> fun (Percent c) -> c
        |> should (equalWithin 0.001) 0.0m

    [<Fact>] 
    let``Quirky. APR calculation 1 payment`` () =
        UnitedKingdom.calculateApr (DateTime(2012,10,10)) 50000L<Cent> [| { TransferType = Payment; Date = DateTime(2012, 10, 15); Amount = 51000L<Cent> } |]
        |> ValueOption.defaultValue (Percent 0m)
        |> fun (Percent c) -> c
        |> should (equalWithin 0.001) 324.436m

    [<Fact>] 
    let``Quirky. APR calculation 2 payments`` () =
        UnitedKingdom.calculateApr (DateTime(2012,10,10)) 50000L<Cent> [| { TransferType = Payment; Date = DateTime(2012,11,10); Amount = 27000L<Cent> }; { TransferType = Payment; Date = DateTime(2012,12,10); Amount = 27000L<Cent> } |]
        |> ValueOption.defaultValue (Percent 0m)
        |> fun (Percent c) -> c
        |> should (equalWithin 0.001) 84.63m

    let calculateUkOld advanceAmount payment paymentCount intervalSchedule advanceDate =
        let payments = intervalSchedule |> Schedule.generate paymentCount Schedule.Forward |> Array.map(fun dt -> { TransferType = Payment; Date = dt; Amount = payment })
        UnitedKingdom.``APR Calculation`` advanceDate (decimal advanceAmount) (payments |> Array.map(fun t -> t.Date, decimal t.Amount))

    let calculateUk advanceAmount payment paymentCount intervalSchedule advanceDate =
        let payments = intervalSchedule |> Schedule.generate paymentCount Schedule.Forward |> Array.map(fun dt -> { TransferType = Payment; Date = dt; Amount = payment })
        UnitedKingdom.calculateApr advanceDate advanceAmount payments
    let calculateUs advanceAmount payment paymentCount intervalSchedule advanceDate =
        let consummationDate = advanceDate
        let firstFinanceChargeEarnedDate = consummationDate
        let advances = [| { TransferType = Advance; Date = consummationDate; Amount = advanceAmount } |]
        let payments = intervalSchedule |> Schedule.generate paymentCount Schedule.Forward |> Array.map(fun dt -> { TransferType = Payment; Date = dt; Amount = payment })
        UsActuarial.generalEquation consummationDate firstFinanceChargeEarnedDate advances payments
        |> Percent.fromDecimal |> Percent.round 2

    [<Fact>]
    let ``UK-old vs UK. Example (c)(1)(i): Monthly payments (regular first period)`` () =
        let actual = calculateUkOld 500000L<Cent> 23000L<Cent> 24 (Monthly (1, MonthlyConfig (1978, 2, 10<TrackingDay>))) (DateTime(1978, 1, 10))
        let expected = calculateUk 500000L<Cent> 23000L<Cent> 24 (Monthly (1, MonthlyConfig (1978, 2, 10<TrackingDay>))) (DateTime(1978, 1, 10)) |> ValueOption.defaultValue (Percent 0m) |> fun (Percent c) -> c
        actual |> should (equalWithin 0.0001) expected

    [<Fact>]
    let ``UK-old vs UK. Example (c)(1)(ii): Monthly payments (long first period)`` () =
        let actual = calculateUkOld 600000L<Cent> 20000L<Cent> 36 (Monthly (1, MonthlyConfig (1978, 4, 1<TrackingDay>))) (DateTime(1978, 2, 10))
        let expected = calculateUk 600000L<Cent> 20000L<Cent> 36 (Monthly (1, MonthlyConfig (1978, 4, 1<TrackingDay>))) (DateTime(1978, 2, 10)) |> ValueOption.defaultValue (Percent 0m) |> fun (Percent c) -> c
        actual |> should (equalWithin 0.0001) expected

    [<Fact>]
    let ``UK-old vs UK. Example (c)(1)(iii): Semimonthly payments (short first period)`` () =
        let actual = calculateUkOld 500000L<Cent> 21917L<Cent> 24 (SemiMonthly (SemiMonthlyConfig (1978, 3, 1<TrackingDay>, 16<TrackingDay>))) (DateTime(1978, 2, 23))
        let expected = calculateUk 500000L<Cent> 21917L<Cent> 24 (SemiMonthly (SemiMonthlyConfig (1978, 3, 1<TrackingDay>, 16<TrackingDay>))) (DateTime(1978, 2, 23)) |> ValueOption.defaultValue (Percent 0m) |> fun (Percent c) -> c
        actual |> should (equalWithin 0.0001) expected

    [<Fact>]
    let ``UK-old vs UK. Example (c)(1)(iv): Quarterly payments (long first period)`` () =
        let actual = calculateUkOld 1000000L<Cent> 38500L<Cent> 40 (Monthly (3, MonthlyConfig (1978, 10, 1<TrackingDay>))) (DateTime(1978, 5, 23))
        let expected = calculateUk 1000000L<Cent> 38500L<Cent> 40 (Monthly (3, MonthlyConfig (1978, 10, 1<TrackingDay>))) (DateTime(1978, 5, 23)) |> ValueOption.defaultValue (Percent 0m) |> fun (Percent c) -> c
        actual |> should (equalWithin 0.0001) expected

    [<Fact>]
    let ``UK-old vs UK. Example (c)(1)(v): Weekly payments (long first period)`` () =
        let actual = calculateUkOld 50000L<Cent> 1760L<Cent> 30 (Weekly (1, DateTime(1978, 4, 21))) (DateTime(1978, 3, 20))
        let expected = calculateUk 50000L<Cent> 1760L<Cent> 30 (Weekly (1, DateTime(1978, 4, 21))) (DateTime(1978, 3, 20)) |> ValueOption.defaultValue (Percent 0m) |> fun (Percent c) -> c
        actual |> should (equalWithin 0.0001) expected