# Business Cases Index

This document provides an index of business cases supported by FSharp.Finance.Personal, organized by domain and use case. Each business case is presented for analytical purposes only and does not constitute financial advice.

## Core Personal Finance (Current Implementation)

### Personal Loans & Consumer Credit
**Purpose**: Calculate APR, payment schedules, and amortization for personal loans, mortgages, hire purchase agreements, and consumer credit products.

**Key Functions/Types**:
- `Apr.calculate` - APR calculations using EU/UK/US methods
- `Amortisation.generate` - Payment schedule generation
- `Scheduling.generate` - Custom payment scheduling
- `Interest.calculate` - Interest calculations (actuarial, add-on)

**Example Scripts**:
- `exampleAprEu.fsx` - EU APR calculations
- `exampleAprUk.fsx` - UK APR calculations  
- `exampleAprUs.fsx` - US APR calculations
- `exampleAmortisation.fsx` - Amortization schedules
- `examplePaymentSchedule.fsx` - Payment scheduling

**Jurisdictional Notes**: 
- EU method: Directive 2008/48/EC compliance
- UK method: FCA regulations (currently identical to EU)
- US method: CFPB actuarial method implementation

## Extended Business Cases (Roadmap/Future)

### Trade Credit Finance
**Purpose**: Short-term financing against trade receivables, including supply chain finance and working capital optimization.

**Key Functions/Types**: *Future implementation*
- Trade credit terms analysis
- Early payment discount calculations
- Working capital impact assessment

**Example Scripts**: *To be developed in PR #1*

**Jurisdictional Notes**: Subject to commercial lending regulations; placeholder for international trade finance standards.

### Invoice Factoring & Discounting
**Purpose**: Immediate cash flow through selling or borrowing against invoice receivables.

**Key Functions/Types**: *Future implementation*
- Factoring cost calculations
- Recourse vs. non-recourse analysis
- Discount rate determinations

**Example Scripts**: *To be developed in PR #1*

**Jurisdictional Notes**: Regulated under commercial financing laws; assignment of receivables subject to local contract law.

### Salary Advance & Earned Wage Access
**Purpose**: Employee salary advance calculations and earned wage access programs.

**Key Functions/Types**: *Future implementation*
- Earned wage calculations
- Advance fee structures
- Payroll integration scenarios

**Example Scripts**: *To be developed in PR #2*

**Jurisdictional Notes**: Subject to employment law and wage protection regulations; varies significantly by jurisdiction.

### XIRR (Extended Internal Rate of Return)
**Purpose**: Complex cash flow analysis for irregular payment schedules and investment scenarios.

**Key Functions/Types**: *Future implementation*
- Irregular cash flow IRR calculations
- Investment return analysis
- Portfolio performance metrics

**Example Scripts**: *To be developed in PR #3*

**Jurisdictional Notes**: Standard financial calculation method; reporting requirements may vary by jurisdiction.

### Equipment Finance & Leasing
**Purpose**: Asset-based financing calculations including lease vs. buy analysis and depreciation considerations.

**Key Functions/Types**: *Future implementation*
- Lease payment calculations
- Residual value analysis
- Tax depreciation integration (MACRS, etc.)

**Example Scripts**: *Future development*

**Jurisdictional Notes**: Tax depreciation rules vary (US MACRS, UK allowances, etc.); lease accounting standards may apply.

## Cross-Reference Table

| Business Case | Core Modules | Example Scripts | Jurisdictional Elements |
|---------------|--------------|-----------------|------------------------|
| Personal Loans | Apr, Amortisation, Interest | exampleApr*.fsx, exampleAmortisation.fsx | EU/UK/US APR methods |
| Consumer Credit | Scheduling, Quotes, Calculation | examplePaymentSchedule.fsx | FCA/CFPB compliance |
| Trade Credit | *Future* | *PR #1* | Commercial lending regs |
| Invoice Factoring | *Future* | *PR #1* | Assignment law |
| Salary Advance | *Future* | *PR #2* | Employment law |
| XIRR Analysis | *Future* | *PR #3* | Standard calc method |
| Equipment Finance | *Future* | *Future* | Tax depreciation rules |

## Important Disclaimers

**Analytical Use Only**: All calculations provided by this library are for analytical purposes only and do not constitute financial, legal, or regulatory advice.

**User Responsibility**: Users are responsible for:
- Ensuring compliance with applicable regulations in their jurisdiction
- Validating calculation results independently
- Obtaining appropriate licenses for commercial use
- Meeting all statutory and regulatory requirements

**Jurisdictional Variations**: Regulatory requirements, calculation methods, and compliance standards vary significantly across jurisdictions. The examples provided use placeholder jurisdictional implementations that may not reflect current local requirements.

**No Warranty**: This library is provided without warranty. Users must independently validate all calculations and ensure regulatory compliance.

For detailed usage conditions and compliance considerations, see [USAGE_AND_COMPLIANCE.md](USAGE_AND_COMPLIANCE.md).