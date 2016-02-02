## v1.11.0 / 2016 Feb 2

> Added the ability to remove fields from `FieldUtils` so that specific fields
> can be ignored without forcing the user to add all the others in the 
> collection.

* **Update** `FieldUtils` now supports removing specific fields

## v1.10.6 / 2016 Jan 27

> Cleanup from the date formatting madness.

* **Update** `LoanDataUtils` now has a configurable datetime formatter

## v1.10.5 / 2016 Jan 27

> Trying date-time formatter again

## v1.10.4 / 2016 Jan 27

> Restoring Loan.LastModified code from master

## v1.10.3 / 2016 Jan 27

> Fix bug in `AysncEventReporter` where it would retry events that were 
> `ACCEPTED` instead of `OK`

* **FIX** `AysncEventReporter` now has a set of `success` http response codes:
HttpStatusCode.OK, HttpStatusCode.Accepted, HttpStatusCode.Continue

## v1.10.2 / 2016 Jan 27

> Non-release debug version for tracking the LastModified problem

## v1.10.1 / 2016 Jan 26

> This release adds defensive code around getting the value of loan.LastModified.
> The value returned is not always valid, so the defensive code will do 
> polling and attempt to correct.

* **Fix** New logic for getting last modified in 
`LoanDataUtils.GetBestGuessLastModified(Loan loan)`

```c#
[GuaranteedRate.Sextant "1.10.1"]
```

## v1.10.0 / 2016 Jan 25

> This release corrects the shutdown mechanics of `AsyncEventReporter` so that
> callers are able to shutdown without losing data that is on the queue.

* **Fix** `AsyncEventReporter.Shutdown()` to shutdown cleanly.  A side effect
of this change is that this method now *BLOCKS* until the reporter has shutdown


```c#
[GuaranteedRate.Sextant "1.10.0"]
```

## v1.9.1 / 2016 Jan 11

> Minor bugfix bump - missed logging if a POST from the `AsyncEventReporter` 
> failed

* **Fix** - Logging in `AsyncEventReporter`

## v1.9.0 / 2016 Jan 11

> This release updates the `FieldUtils` and `LoanDetails` to itterate over the
> correct index for index based multi-fields.  It also updates the 
> borrower-pair extraction to provide a more complete data set.

* **Fix** - `LoanDataUtils.ExtractMilestones` had direct access to a field 
instead of using ParseField
* **Fix** - Bug with marking the primary pair
* **Update** - Greatly expanded the list of fields in `BORROWER_PAIR_FIELDS`
* **Update** - Switched all the Lists for Sets in `FieldUtils` 
* **Update** - Changed the timeout for POST operations from 20s to 45s in
`AsyncEventReporter` to handle increased insertion time
* **Update** - Added retries to `AsyncEventReporter` to increase relability
* **Fix** -`Loggly` class would fill the message queue with errors if no url
was configured.  Now logging disabled if no url configued.
* **Add** `GuaranteedRate.Util.IndexFields` project to hammer out the last of 
the index fields without boundaries.

## v1.8.0 / 2016 Jan 15

> Add extention class to extend the session object for simplified manipulation of the server's registered users

* **Add** - `UserUtils.cs`
* **Add** - 'GuaranteedRate.Examples.UserUtils' for `UserUtils.cs`

## v1.7.1 / 2015 Nov 19

> Add Function to get list of reportable virtual fields

* **Add** - `ReportableVirtualFields` to `FieldUtils.cs`

## v1.7.0 / 2015 Oct 29

> Add Pipeline and Report querying to `Reports.cs`
> Also contains unused improvements for field extraction

* **Add** - `LoansAndLastModifiedPipeline` to `Reports.cs`
* **Add** - `LoansAndLastModifiedReport` to `Reports.cs`
* **Update** - All reports to support just start time
* **Update** - `FieldUtils.cs` with more multi-field sorting 
(unused by `LoanExtractor.cs`)

## v1.6.0 / 2015 Oct 17

> Add the userId of the session to the Properties associated with a loan in 
> `LoanDataUtils`

* **Update** - `LoanDataUtils.ExtractProperties` to include the UserId associated
with the current session
* **JIRA** - [POL-244](http://jira.guaranteedrate.com/browse/POL-244)

## v1.5.1 / 2015 Oct 12

> Update to FieldUtils to allow selection of individual fields, not
> just field description collections

* **Add** - FieldUtils.AddFieldCollection to add a single field to the cached list
* **Update** - FieldUtils.GetAllFieldIds to support adding a single field to the 
cached list
* **Update** - FieldUtils.GetAllFieldIds to support adding a single field to the 
cached list
* **ADD** - FieldUtils.AddFieldDescriptors to support adding a single field to the 
cached list
* **Update** - FieldUtils.GetAllFieldDescriptors to support adding a single field 
to the cached list


## v1.5.0 / 2015 Oct 12

> Major rewrite of collection of multi-indexed fields.
> Multi-indexed fields are now unrolled into multiple fields.
> Where possible the field name unrolling occurs in FieldUtils, but when the 
> keys are based on the loan, they are done in LoanDataUtils

* **Update** - FieldUtils now unrolls Milestone indexed mutlifield ids
* **Update** - FieldUtils now unrolls Role indexed mutlifield ids
* **Update** - LoanDataUtils now extracts Document indexed multifields
* **Update** - LoanDataUtils now extracts Post Closing indexed multifields
* **Update** - LoanDataUtils now extracts Underwriting indexed multifields
* **Update** - LoanDataUtils now extracts Milestone Task indexed multifields
* **Update** - LoanDataUtils now extracts Milestone indexed multifields
* **Update** - LoanDataUtils now extracts Role indexed multifields
* **JIRA** - [POL-228](http://jira.guaranteedrate.com/browse/POL-228)

## v1.4.0 / 2015 Oct 8

> Improvements to Borrower-Pair collection

* **Update** - FieldUtils.BORROWER_PAIR_FIELDS - Add field "1268", Co-Borrower email
* **JIRA** - [POL-224](http://jira.guaranteedrate.com/browse/POL-224)

## v1.3.4 / 2015 Sept 22

> Defensive coding for everything in LoanDataUtils

## v1.3.3 / 2015 Sept 22

> Bug fix - Not all loans have a last modified timestamp

* **Fix** - `LoanDataUtils.ExtractEverything` to better catch exceptions during field extraction

## v1.3.2 / 2015 Sept 22

> Bug fix - continued problems with borrower pairs

* **Fix** - `LoanDataUtils.ExtractBorrowerPairs` to better encapsulate field extraction

## v1.3.1 / 2015 Sept 18

> Bug fix - if the primary borrower does not have an SSN, do not attempt
> to extract borrower pairs

* **Fix** - `LoanDataUtils.ExtractBorrowerPairs` fix exception when the primary borrower does not have
a SSN

## v1.3.0 / 2015 Sept 18

> Created `Reporting` class to run common reports

* **Add** - `Reporting.LoansLastModifiedBetween` which returns a list of loans last modified between 2 dates 

## v1.2.2 / 2015 Sept 18

> Fixed bug in IniConfig.  Kyes are converted to lowercase

* **Fixed** - `IniConfig.GetValue` call `ToLower()` on the requested config key

## v1.2.1 / 2015 Sept 18

> Fixed bug in IniConfig.  C# is not Java

* **Fixed** - `IniConfig.GetValue` Corrected logic for accessing dictionaries (Java Maps return null). 

## v1.2.0 / 2015 Sept 17

> Improved loan data extraction.  Especially regarding borrower pairs

* **Added** - `LoanDataUtils.ExtractBorrowerPairs` which returns useful borrower pair information
* **Added** - `LoanDataUtils.ExtractProperties` which returns useful loan property information
* **Updated** - `LoanDataUtils.ExtractLoanFields` to call the 2 new functions

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
