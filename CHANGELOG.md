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
