## v1.5.0 / 2015 Oct 12

> Major rewrite of collection of multi-indexed fields.
> Multi-indexed fields are now unrolled into multiple fields.
> Where possible the field name unrolling occurs in FieldUtils, but when the 
> keys are based on the loan, they are done in LoanDataUtils

* **Update** FieldUtils now unrolls Milestone indexed mutlifield ids
* **Update** FieldUtils now unrolls Role indexed mutlifield ids
* **Update** LoanDataUtils now extracts Document indexed multifields
* **Update** LoanDataUtils now extracts Post Closing indexed multifields
* **Update** LoanDataUtils now extracts Underwriting indexed multifields
* **Update** LoanDataUtils now extracts Milestone Task indexed multifields
* **Update** LoanDataUtils now extracts Milestone indexed multifields
* **Update** LoanDataUtils now extracts Role indexed multifields
* **JIRA** [POL-228](http://jira.guaranteedrate.com/browse/POL-228)

## v1.4.0 / 2015 Oct 8

> Improvements to Borrower-Pair collection

* **Update** FieldUtils.BORROWER_PAIR_FIELDS - Add field "1268", Co-Borrower email
* **JIRA** [POL-224](http://jira.guaranteedrate.com/browse/POL-224)

## v1.3.4 / 2015 Sept 22

> Defensive coding for everything in LoanDataUtils

## v1.3.3 / 2015 Sept 22

> Bug fix - Not all loans have a last modified timestamp

* **Fix** `LoanDataUtils.ExtractEverything` to better catch exceptions during field extraction

## v1.3.2 / 2015 Sept 22

> Bug fix - continued problems with borrower pairs

* **Fix** `LoanDataUtils.ExtractBorrowerPairs` to better encapsulate field extraction

## v1.3.1 / 2015 Sept 18

> Bug fix - if the primary borrower does not have an SSN, do not attempt
> to extract borrower pairs

* **Fix** `LoanDataUtils.ExtractBorrowerPairs` fix exception when the primary borrower does not have
a SSN

## v1.3.0 / 2015 Sept 18

> Created `Reporting` class to run common reports

* **Add** `Reporting.LoansLastModifiedBetween` which returns a list of loans last modified between 2 dates 

## v1.2.2 / 2015 Sept 18

> Fixed bug in IniConfig.  Kyes are converted to lowercase

* **Fixed** `IniConfig.GetValue` call `ToLower()` on the requested config key

## v1.2.1 / 2015 Sept 18

> Fixed bug in IniConfig.  C# is not Java

* **Fixed** `IniConfig.GetValue` Corrected logic for accessing dictionaries (Java Maps return null). 

## v1.2.0 / 2015 Sept 17

> Improved loan data extraction.  Especially regarding borrower pairs

* **Added** `LoanDataUtils.ExtractBorrowerPairs` which returns useful borrower pair information
* **Added** `LoanDataUtils.ExtractProperties` which returns useful loan property information
* **Updated** `LoanDataUtils.ExtractLoanFields` to call the 2 new functions

## v1.1.0 / 2015 Sept 4

> Added `LoanDataUtils.ExtractMilestones` which returns a list of milestone data.
> Added `LoanDataUtils.ExtractEverything` which runs all the extraction methods and returns a dictonary of the collections.

## v1.0.1 / 2015 Aug 28

> Update to FieldUtils to make it possible to collect a subset of fields instead of always getting all fields.
> Revised the readme to be more descriptive

## v1.0.0 / 2015 Aug 27

> Initial release.
> This code is being copied and sanitized from an existing internal repo.
> Losing the history is intentional.
