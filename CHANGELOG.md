## v17.1.0.1 / 2017 Feb 23

> This release forces trimming on field ids to prevent Encompass "Field ID not found" errors 

```c#
[GuaranteedRate.Sextant "17.1.0.1"]
```

## v17.1.0.0 / 2017 Feb 08

> This release updates the Encompass SDK to version 17.1 and includes instructions 
> for building your own NuGet package.

* **Update** - Encompass SDK to 17.1.
* **Add** - Encompass Nuget package nuspec and instructions.

```c#
[GuaranteedRate.Sextant "17.1.0.0"]
```

## v16.2.7.1 / 2017 Jan 27

> This bugfix release fixes `AsyncEventReporter`, it considered some http 
> status codes as failures, which resulted in extra retries

* **Fix** - `AsyncEventReporter` now considers 201 & 204 successful responses

```c#
[GuaranteedRate.Sextant "16.2.7.1"]
```

## v16.2.7.0 / 2017 Jan 20

> This release adds functionality to lookup a loan guid by loan number.

* **Add** - Expose a function to return the loan guid for a given loan number.

```c#
[GuaranteedRate.Sextant "16.2.7.0"]
```

## v16.2.6.0 / 2017 Jan 17

> This release adds various pieces of Funding and CD Fees to the set of data
> extracted by `LoanDataUtils`.  It appears that some or all of the data is 
> duplicated elsewhere, but this addition makes the data more accessable

* **Add** - LoanFee data to the set of data extracted in `LoanDataUtils`

```c#
[GuaranteedRate.Sextant "16.2.6.0"]
```

## v16.2.5.3 / 2016 

> This bugfix release changes the way items are added to a dictionary in 
> `LoanDataUtils`.  The code has been changed from dictionary.Add(key, value)
> to dictionary[key] = value.  We are seeing errors in production where 
> duplicate keys are being added to the dictionary, which causes an exception.
>
> Since the keys coming from `FieldUtils` are contained in Sets, it appears
> that keys are being defined in multiple field types.

* **Update**  - Changed the dictionary insertions in `LoanDataUtils` to be safe
in case of duplicates.

```c#
[GuaranteedRate.Sextant "16.2.5.3"]

```

## v16.2.5.2 / 2016 Sep 27
> Fix typo in Loggly.cs

* **Update**  - Loggly.cs  

```c#
[GuaranteedRate.Sextant "16.2.5.2"]

```

## v16.2.5.1 / 2016 Sep 27
> Add loanId and loanGuid to the exception.

* **Update**  - LoanDataUtils 

```c#
[GuaranteedRate.Sextant "16.2.5.1"]

```
## v16.2.5.0 / 2016 Sep 20
> Added Init(string) to the config.
> Supports loading from known strings.  Useful for tests.

* **Update**  - JsonEncompassConfig
* **Update**  - IniConfig
* **Update**  - IEncompassConfig

```c#
[GuaranteedRate.Sextant "16.2.5.0"]
```

## v16.2.4.2 / 2016 Sep 15

>Increased error granularity in field extraction.

* **Update**  - LoanDataUtils.cs will return clearer error messages. 



## v16.2.4.1 / 2016 Sep 15

>Fixed an issue with a flipped boolean in json config init.

* **Update**  - JsonEncompassConfig.cs was returning false on success fix to true. 


## v16.2.4.0 / 2016 Sep 13

>Fixed an issue with the json config where it was not safe to call Init() 
>multiple times
>Added the ability to have Loggly load tags from config rather than be explicitly 
>passed in.

* **Update**  - Loggly.cs overloaded init method that doesn't do a force reload

```c#
[GuaranteedRate.Sextant "16.2.4.0"]
```
## v16.2.3.0 / 2016 Sep 9
> Added the JsonEncompassConfig to support using JSON for configuration files
> and added support for Loggly.Fatal errors for when things go really wrong.

* **Add**  - JsonEncompassConfig
* **Add**  - Loggly.Fatal

```c#
[GuaranteedRate.Sextant "16.2.3.0"]
```


## v16.2.2.0 / 2016 Aug 10

> Added the server URI and smart client to the list of fields collected by
> `LoanDataUtils`, which is useful for users supporting mutliple Encompass 
> Environments

* **Add** - `Session.ServerURI` as `SessionServerURI` to the Loan Dictonary 
created by LoanDataUtils.
* **Add** - `smart client id` as `SessionSmartClientId` to the Loan Dictonary 
created by LoanDataUtils.  This value is derived from the ServerURI.

```c#
[GuaranteedRate.Sextant "16.2.2.0"]
```

## v16.2.1.0 / 2016 Aug 9

> This version removes the dependency on the EncompassSDK.
> We did this because the SDK must be installed on the target machine in all 
> cases, and having a nuget dependency only intruduces potential dependency 
> conflicts

* **Update** - Removed the EncompassSDK nuget package, and added hardcoded
references to the installed EncompassSDK

```c#
[GuaranteedRate.Sextant "16.2.1.0"]
```

## v16.2.0.0 / 2016 July 7

> Changing the versioning scheme to match Ellie Mae.
> Now the project's first 2 partitions will match the `EncompassSDK` version 
> used by the project.  Because the SDK versions are incompatable this makes
> sense as a major version indicator.  The 3rd and 4th numbers will be used to
> indicate major updates and bugfixes respectively.

* **Update** - Updated to EncompassSDK 16.2
* **Update** - Changed project versioning scheme

```c#
[GuaranteedRate.Sextant "16.2.0.0"]
```

## v1.14.0 / 2016 June 10

> Merge screwup - lots of code in thsi release that was never merged to Master.
> This release adds `SecureAsyncEventReporter` which allows posting data to an
> endpoint that requires an `Authorization` token.

* **Add** - `SecureAsyncEventReporter` which will add an `Authorization` token
to the header of the events being posted.
* **Add** - Methods for extracting user/licensing info in `UserUtils`

```c#
[GuaranteedRate.Sextant "1.14.0"]
```

## v1.13.3 / 2016 Jun 09

> Added new interface to support IoC containers, minor bugfix, added AddMeter functionality.

* **Add** - Added AddMeter to datadog reporter
* **Fix** - Correct spelling in datadog reporter.  `guage` should be `gauge`.  Add additional method to support old spelling.  Mark old spelling "obsolete."


```c#
[GuaranteedRate.Sextant "1.13.3"]
```

## v1.13.2 / 2016 May 17

> Bug in `LoanDataUtils` - `PostClosingConditions` were being cast as `UnderwritingConditions`

* **Fix** - Fixed invalid object casting in `LoanDataUtils`

```c#
[GuaranteedRate.Sextant "1.13.2"]
```

## v1.13.1 / 2016 May 2

> Switched back to `EncompassSDK.Complete`, `EncompassSDK.Standard` is
> insufficient for SDK apps.

* **Fix** - Switched back from EncompassSDK.Standard to EncompassSDK.Complete.  
Standard isn't sufficient for standalone apps.

```c#
[GuaranteedRate.Sextant "1.13.1"]
```

## v1.13.0 / 2016 April 22

> Collection of improvements to handle Encompass Performance issues being 
> experienced by GuaranteedRate.

* **Fix** - Switched from EncompassSDK.Complete to EncompassSDK.Standard.  
Complete is unneccessary.
* **Add** - Added optional init() method to Loggly logger that will auto 
configure itself from config
* **Add** - Added ability to turn logging levels on/off

```c#
[GuaranteedRate.Sextant "1.13.0"]
```

## v1.12.2 / 2016 March 29

> Bugfix release to correct logic that identifies the primary borrower pair.
> If the loan object passed in to LoanDataUtils.ExtractEverything did not 
> have the primary pair as current pair, the current pair would be incorrectly
> listed as the primary pair.
> Additionally added a convience method to open a loan by loan number

* **Fix** - Identification of primary borrower pair on a loan
* **Add** - Convience method in SessionUtils to open a loan by LoanNumber instead
of GUID

```c#
[GuaranteedRate.Sextant "1.12.2"]
```

## v1.12.1 / 2016 March 21

> Updating to Encompass SDK 15.2.  No other changes.

```c#
[GuaranteedRate.Sextant "1.12.1"]
```

## v1.12.0 / 2016 March 17

> Adding the ability to collect active directory and machine information
> Adding AD and machine information to ExtractEverything method data return

```c#
[GuaranteedRate.Sextant "1.12.0"]
```

## v1.11.3 / 2016 Feb 23

> Fixing bug in `AsyncEventReporter` - the function `ExtraSetup` wasn't marked
> as override

* **Fix** - Java dev makes C# mistake...

```c#
[GuaranteedRate.Sextant "1.11.3"]
```

## v1.11.2 / 2016 Feb 23

> Fixing bug in `AsyncEventReporter` - the function `ExtraSetup` wasn't being
> called.

* **Fix** - `AsyncEventReporter` now calls `ExtraSetup` when setting up an event

```c#
[GuaranteedRate.Sextant "1.11.2"]
```

## v1.11.1 / 2016 Feb 2

> Updated logging in `LoanDataUtils` to include the `loanNumber` where the 
> exception occured

* **Update** - `LoanDataUtils` logging to include the loan number

```c#
[GuaranteedRate.Sextant "1.11.1"]
```

## v1.11.0 / 2016 Feb 2

> Added the ability to remove fields from `FieldUtils` so that specific fields
> can be ignored without forcing the user to add all the others in the 
> collection.

* **Update** - `FieldUtils` now supports removing specific fields

```c#
[GuaranteedRate.Sextant "1.11.0"]
```

## v1.10.6 / 2016 Jan 27

> Cleanup from the date formatting madness.

* **Update** - `LoanDataUtils` now has a configurable datetime formatter

```c#
[GuaranteedRate.Sextant "1.10.6"]
```

## v1.10.5 / 2016 Jan 27

> Trying date-time formatter again

```c#
[GuaranteedRate.Sextant "1.10.5"]
```

## v1.10.4 / 2016 Jan 27

> Restoring Loan.LastModified code from master

```c#
[GuaranteedRate.Sextant "1.10.4"]
```

## v1.10.3 / 2016 Jan 27

> Fix bug in `AysncEventReporter` where it would retry events that were 
> `ACCEPTED` instead of `OK`

* **FIX** - `AysncEventReporter` now has a set of `success` http response codes:
HttpStatusCode.OK, HttpStatusCode.Accepted, HttpStatusCode.Continue

```c#
[GuaranteedRate.Sextant "1.10.3"]
```

## v1.10.2 / 2016 Jan 27

> Non-release debug version for tracking the LastModified problem

```c#
[GuaranteedRate.Sextant "1.10.2"]
```

## v1.10.1 / 2016 Jan 26

> This release adds defensive code around getting the value of loan.LastModified.
> The value returned is not always valid, so the defensive code will do 
> polling and attempt to correct.

* **Fix** - New logic for getting last modified in 
`LoanDataUtils.GetBestGuessLastModified(Loan loan)`

```c#
[GuaranteedRate.Sextant "1.10.1"]
```

## v1.10.0 / 2016 Jan 25

> This release corrects the shutdown mechanics of `AsyncEventReporter` so that
> callers are able to shutdown without losing data that is on the queue.

* **Fix** - `AsyncEventReporter.Shutdown()` to shutdown cleanly.  A side effect
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
