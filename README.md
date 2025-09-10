# FSharp.Finance.Personal

Personal-finance functions written in F#

This library helps you verify your personal finance events, including verifying and creating payment plans, repayment schedules 
and interest calculations for things like mortgages, loans, hire purchases, debt consolidations, car loans, line of credit, etc. 

Initial features:

> APR calculation

> amortisation

This library operates partially in areas where business is regulated by various regulators.
Though every care has been taken to ensure the accuracy of the results, please independently validate the figures produced by it.
It is not audited or validated by any of the regulators.

If you have any suggestions or corrections, please feel free to comment or create a pull request.

For commercial use the user might need an operating license and to fulfil various statutory and regulatory requirements,
none of which are conferred by the use of this library.

## Business Case Index & Usage Conditions

This library supports multiple personal finance business cases, each with specific analytical capabilities and jurisdictional considerations. Comprehensive documentation is available covering business case domains, usage conditions, and compliance requirements:

- **[Business Cases Index](docs/BUSINESS_CASES_INDEX.md)** - Complete index of supported business cases including personal loans, trade credit, invoice factoring, salary advances, XIRR analysis, and equipment finance, with cross-references to example scripts and jurisdictional notes.

- **[Usage and Compliance](docs/USAGE_AND_COMPLIANCE.md)** - Comprehensive usage conditions, regulatory disclaimers, input validation requirements, and production deployment guidelines.

**Key Points**:
- All functionality is **analytical only** and does not constitute financial advice
- Jurisdictional implementations include **placeholders** (UK APR, US MACRS depreciation) that require validation for specific use cases
- Tax shield calculations are **opt-in** and require explicit activation
- Users are responsible for ensuring regulatory compliance and obtaining appropriate licenses for commercial use

NuGet package: https://www.nuget.org/packages/FSharp.Finance.Personal/

Documentation: https://simontreanor.dev/FSharp.Finance.Personal/
