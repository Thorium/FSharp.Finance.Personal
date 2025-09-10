# Usage and Compliance

*Document created: December 10, 2024*

This document outlines usage conditions, compliance considerations, and important disclaimers for FSharp.Finance.Personal. All functionality is provided for analytical purposes only.

## Fundamental Usage Conditions

### Analytical-Only Nature
FSharp.Finance.Personal is designed exclusively for analytical calculations and financial modeling. It **does not**:
- Provide financial advice or recommendations
- Constitute regulated financial services
- Replace professional financial or legal consultation
- Guarantee regulatory compliance in any jurisdiction

### User Responsibilities
Users are solely responsible for:
- Independent validation of all calculation results
- Ensuring compliance with applicable local regulations
- Obtaining necessary licenses for commercial use
- Meeting statutory and regulatory requirements
- Proper interpretation and application of results

## Jurisdictional Scope and Limitations

### Implemented Jurisdictions
The library currently provides calculation methods for:
- **European Union**: Directive 2008/48/EC APR calculations
- **United Kingdom**: FCA-compliant APR methods (currently identical to EU)
- **United States**: CFPB actuarial method implementations

### Jurisdictional Disclaimers
- Regulatory requirements vary significantly across jurisdictions
- Examples use placeholder implementations that may not reflect current local requirements
- Users must verify compliance with applicable local laws and regulations
- No representation is made regarding accuracy for any specific jurisdiction

### Regulatory Considerations
This library operates in areas subject to regulation by various authorities including:
- Financial conduct authorities (FCA, CFPB, etc.)
- Consumer protection agencies
- Commercial lending regulators
- Tax authorities (for depreciation and tax shield calculations)

## Amount Guidance and Practical Limits

### Minimum Amounts
- **Technical minimum**: 1 cent (0.01 in base currency units)
- **Practical minimum**: Varies by jurisdiction and product type
- **Recommendation**: Validate minimum amounts against local regulatory requirements

### Practical Amount Considerations
- Currency amounts represented as `int64<Cent>` (whole base units only)
- Maximum representable amount: 9,223,372,036,854,775,807 cents
- Practical upper limits depend on calculation complexity and performance requirements
- Large amounts may require extended precision for complex calculations

## Input Validation and Data Quality

### Required Validations
Users must implement appropriate validation for:
- Date ranges and business day calculations
- Principal amounts and payment values
- Interest rates and fee structures
- Payment frequency and scheduling parameters
- Currency precision and rounding requirements

### Data Quality Standards
- All monetary amounts must be in whole base currency units (cents/pence)
- Dates must exclude time components for consistency
- Interest rates should be validated for reasonableness
- Payment schedules must be logically consistent

## Rounding and Precision Handling

### Rounding Philosophy
- All calculations use whole base currency units to ensure customer-visible amounts match calculation inputs
- Rounding errors are handled explicitly rather than accumulated
- Intermediate calculations maintain precision while final outputs are rounded to whole units

### Precision Considerations
- Interest calculations use decimal precision internally
- Final amounts are always rounded to whole base currency units
- APR calculations follow jurisdiction-specific rounding rules
- Users should implement consistent rounding policies

## Tax Shield Integration

### Opt-In Approach
Tax considerations (depreciation, interest deductions, etc.) are implemented as opt-in features:
- Default calculations exclude tax effects
- Tax shield calculations require explicit activation
- Multiple jurisdiction tax rules supported (US MACRS, UK allowances, etc.)

### Tax Disclaimers
- Tax calculations are illustrative only
- Users must consult tax professionals for actual tax implications
- Tax rules change frequently and vary by jurisdiction
- Library implementation may not reflect current tax law

## Regulatory Disclaimers and Limitations

### No Regulatory Approval
- This library is not audited or validated by financial regulators
- No warranty provided for regulatory compliance
- Users must independently verify compliance requirements
- Commercial use may require operating licenses and regulatory approval

### Limitation of Liability
- Library provided "as-is" without warranty of any kind
- No guarantee of accuracy, completeness, or fitness for purpose
- Users assume all risks associated with use
- Authors disclaim liability for any damages arising from use

## Cashflow Sign Orientation

### Convention Standards
- **Payments from borrower**: Positive values
- **Advances to borrower**: Negative values  
- **Fees and charges**: Positive values (added to borrower obligation)
- **Interest accrual**: Positive values (added to balance)

### Consistency Requirements
- All example scripts follow consistent sign conventions
- Users should maintain consistent orientation across calculations
- Mixed sign conventions may produce incorrect results

## Performance and Scalability

### Performance Characteristics
- Optimized for typical personal finance calculation volumes
- Complex amortization schedules may require significant computation time
- Large payment schedules (>1000 payments) should be tested for performance
- Memory usage scales with calculation complexity

### Scalability Considerations
- Suitable for individual calculation scenarios
- Batch processing capabilities depend on specific use case
- High-volume production use should be performance tested
- Consider caching for repeated similar calculations

## Security and PII Considerations

### Data Handling
- Library performs calculations only - does not store data
- Users responsible for secure handling of financial data
- No logging or persistence of calculation inputs or results
- Consider data classification requirements for production use

### Privacy Considerations
- Avoid logging personally identifiable information (PII)
- Implement appropriate data retention policies
- Consider GDPR, CCPA, and other privacy regulations
- Secure disposal of sensitive calculation data

## Production Deployment Checklist

### Pre-Deployment Validation
- [ ] Independent verification of calculation accuracy
- [ ] Regulatory compliance review completed
- [ ] Input validation and error handling implemented
- [ ] Performance testing for expected load completed
- [ ] Security review of data handling procedures
- [ ] Documentation of calculation methodologies
- [ ] Staff training on library limitations completed

### Ongoing Monitoring
- [ ] Regular accuracy testing against known benchmarks
- [ ] Monitoring for regulatory changes affecting calculations
- [ ] Performance monitoring in production environment
- [ ] Regular review of calculation parameters and assumptions

## Development Roadmap

### Current Status (v2.5.5)
- Core personal finance calculations (APR, amortization)
- EU/UK/US jurisdictional implementations
- Basic fee and charge handling
- Payment scheduling capabilities

### Planned Enhancements
- Trade credit and invoice factoring (PR #1)
- Salary advance calculations (PR #2)
- XIRR and complex cashflow analysis (PR #3)
- Enhanced tax shield calculations
- Additional jurisdictional implementations

### Future Considerations
- Real-time rate updates integration
- Enhanced validation frameworks
- Extended regulatory compliance features
- Performance optimization for high-volume scenarios

## Business Case Domain Separation

### Personal Finance Domain
- Consumer loans, mortgages, credit cards
- Regulated under consumer protection laws
- Individual borrower focus
- Standardized APR calculations

### Commercial Finance Domain
- Trade credit, invoice factoring, equipment finance
- Subject to commercial lending regulations
- Business borrower focus
- More complex fee structures

### Employment Finance Domain
- Salary advances, earned wage access
- Employment law considerations
- Payroll system integration
- Employee protection focus

Each domain has distinct regulatory requirements, calculation methods, and compliance considerations.

## License and Warranty Statement

### License Terms
This software is provided under the terms specified in the LICENSE file. Users are responsible for reviewing and complying with all license terms.

### Warranty Disclaimer
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

### Professional Advice Recommendation
Users should consult qualified financial, legal, and regulatory professionals before implementing any financial calculations in production environments or making business decisions based on library outputs.

---

For business case details and domain-specific considerations, see [BUSINESS_CASES_INDEX.md](BUSINESS_CASES_INDEX.md).

*This document should be reviewed regularly and updated to reflect current regulatory requirements and library capabilities.*