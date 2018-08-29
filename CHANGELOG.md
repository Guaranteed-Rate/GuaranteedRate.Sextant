## v18.2.3.1 / 2018 Aug 29

* **Update** - Upgraded Serilog.Loggly, Loggly and Loggly config nuget packates in order to resolve logging breaking

```csharp
[GuaranteedRate.Sextant "18.2.3.1"]
```

## v18.2.3.0 / 2018 July 18

* **Update** - LOS-2392: Allow us to not track the host. This is a cost saving measure for Datadog.

```csharp
[GuaranteedRate.Sextant "18.2.3.0"]
```


## v18.2.2.1 / 2018 June 29

* **Update** - YBR-591: fix NPE if Conditon is create with "for borrower pair" set to "all"

```csharp
[GuaranteedRate.Sextant "18.2.2.1"]
```

## v18.2.2.0 / 2018 June 29

* **Update** - YBR-591: extract underwriting conditions data into a robust format

```csharp
[GuaranteedRate.Sextant "18.2.2.0"]
```

## v18.2.1.3 / 2018 June 25

* **Fix**  - Fix field name error in Loan Metadata (LOS-2255)

```csharp
[GuaranteedRate.Sextant "18.2.1.3"]
```

## v18.2.1.2 / 2018 June 25

* **Add**  - create reporting method to retrieve LoanMetadata rather than retrieving entire loan (LOS-2255)

```csharp
[GuaranteedRate.Sextant "18.2.1.2"]
```

## v18.2.1.1 / 2018 June 19

* **Fix**  - Fix NPE when uw-conditigiton does not have a role assigned (YBR-591)
* **Update** - throw an exception including the condition title if error occurs while reading uw conditions (YBR-591)
* **Add**  - capture additional data about underwriting conditions into uw-conditions-metadata field (YBR-591)

```csharp
[GuaranteedRate.Sextant "18.2.1.1"]
```

## v18.1.0.0 / 2018 March 4

* **Update**  - Upgrade all projects to Encompass 18.1

```csharp
[GuaranteedRate.Sextant "18.1.0.0"]
```

## v17.4.1.7 / 2018 Feb 20

* **Update**  - Ensure date format is provided in Elasticsearch indexes.

```csharp
[GuaranteedRate.Sextant "17.4.1.7"]
```

## v17.4.1.6 / 2018 Feb 20

* **Fix**  - Fix double logging. 
* **Fix**  - Fix field collision.

```csharp
[GuaranteedRate.Sextant "17.4.1.6"]
```

## v17.4.1.5 / 2018 Feb 14

* **Update**  - Ensure logs contain structured data.

```csharp
[GuaranteedRate.Sextant "17.4.1.5"]
```

## v17.4.1.4 / 2018 Feb 13

* **Add**  - Make AddTag static
* **Add**  - Add optional dictionary of tags on setup.

```csharp
[GuaranteedRate.Sextant "17.4.1.4"]
```


## v17.4.1.3 / 2017 Feb 13

* **Fix**  - Fix remaining NRE on debug statements if logging is not configured.
```csharp
[GuaranteedRate.Sextant "17.4.1.3"]
```


## v17.4.1.2 / 2018 Feb 13

* **Update**  - Do not null reference error if logging has not been configured.
```csharp
[GuaranteedRate.Sextant "17.4.1.2"]
```

## v17.4.1.1 / 2018 Feb 13

* **Update**  - Tear out homegrown logging framework and use Serilog instead.
```csharp
[GuaranteedRate.Sextant "17.4.1.1"]
```

## v17.4.0.1 / 2017 Dec 12

* **Update**  - Fix AppNam and Environment values in Elasticsearch.
```csharp
[GuaranteedRate.Sextant "17.4.0.1"]
```


## v17.4.0.0 / 2017 Dec 07

* **Update**  - Move to Encompass 17.4
```csharp
[GuaranteedRate.Sextant "17.4.0.0"]
```



[GuaranteedRate.Sextant "17.4.0.0"]
```
## v17.3.4.0 / 2017 Dec 04

* **Add**  - Additional config-driven tags in Elasticsearch.  
```csharp
[GuaranteedRate.Sextant "17.3.4.0"]
```


## v17.3.3.4 / 2017 Nov 19

* **Fix**  - Unbreak Newtonsoft dependencies.
```csharp
[GuaranteedRate.Sextant "17.3.3.4"]
```


## v17.3.3.2 / 2017 Nov 13

* **Fix**  - Include tags in Elasticsearch appender

```csharp
[GuaranteedRate.Sextant "17.3.3.3"]
```


## v17.3.3.2 / 2017 Nov 9

* **Fix**  - Comment out the attachment collection code from `LoanDataUtils`
It is to slow to use as part of the regular loan serialization.

```csharp
[GuaranteedRate.Sextant "17.3.3.2"]
```

## v17.3.3.1 / 2017 Oct 24

* **Fix**  - Exclude bad fields from middle and end indexes.

```csharp
[GuaranteedRate.Sextant "17.3.3.1"]
```

## v17.3.3.0 / 2017 Oct 15

* **Add**  - Support extraction of loan attachments to the loan data map

```csharp
[GuaranteedRate.Sextant "17.3.3.0"]
```

## v17.3.2.2 / 2017 Oct 13

* **Fix**  - Tweaks to logging to debug issues.

```csharp
[GuaranteedRate.Sextant "17.3.2.2"]
```

## v17.3.2.1 / 2017 Oct 10

* **Add**  -  Expose Shutdown() on IEventReporter so consumers don't have to specify timeout seconds.

```csharp
[GuaranteedRate.Sextant "17.3.2.1"]
```

## v17.3.2.0 / 2017 Oct 10

* **Fix**  -  Numerous fixes to logging.  
* **Add**  -  Expose Shutdown() on logger.  Handle recursive errors.
* **Add**  -  Prmitive file logger (mainly useful in debugging other logging failures).

```csharp
[GuaranteedRate.Sextant "17.3.2.0"]
```

## v17.3.1.1 / 2017 Oct 02

* **Fix**  - force index name toLower because otherwise ElasticSearch dies horribly.
* **Update** - Update Newtonsoft to 10.0.0.0

```csharp
[GuaranteedRate.Sextant "17.3.1.1"]
```
## v17.3.1.0 / 2017 Sep 29

* **Add**  - Add Co-Borr Marital Status to BORROWER_PAIR_FIELDS

```csharp
[GuaranteedRate.Sextant "17.3.1.0"]
```

## v17.3.0.1/ 2017 Sep 26
* **Fix** - Sensible default index name so we aren't creating lots of elasticsearch indexes named after every logger.  
```csharp
[GuaranteedRate.Sextant "17.3.0.1"]
```

## v17.3.0.0 /2017 Sep 18
* **Update** Upgrade to Encompass 17.3

```csharp
[GuaranteedRate.Sextant "17.3.0.0"]
```

## v17.2.4.0 / 2017 Sep 29

* **Add**  - Add Co-Borr Marital Status to BORROWER_PAIR_FIELDS

```csharp
[GuaranteedRate.Sextant "17.2.4.0"]
```

## v17.2.3.0 / 2017 Sep 26
* **Fix** - Sensible default index name so we aren't creating lots of elasticsearch indexes named after every logger.  

```csharp
[GuaranteedRate.Sextant "17.2.3.0"]
```

## v17.2.2.16 / 2017 Sep 07
* **Fix** - Now sending timestamps fully in UTC format
* **Fix** - Fix `ElasticsearchLogAppender` to send timestamps in date format.  Previously we were sending dates as 
a string which was preventing adequate date indexing

```csharp
[GuaranteedRate.Sextant "17.2.2.16"]
```

## v17.2.2.15 / 2017 Sep 05
* **Update** - Add the url of the failing service to the logs

```csharp
[GuaranteedRate.Sextant "17.2.2.15"]
```

## v17.2.2.14 / 2017 Sep 05
* **Fix** - Forcing `AsyncEventReporter` to use `WebRequest` under the hood as `HttpClient` will cause deadlocks in 
systems with asynchronous entry points.

```csharp
[GuaranteedRate.Sextant "17.2.2.14"]
```

## v17.2.2.13 / 2017 Sep 05
* **Fix** - Do not lock the log statement in `Logger.cs`

```csharp
[GuaranteedRate.Sextant "17.2.2.13"]
```

## v17.2.2.12 / 2017 Sep 05
* **Fix** - Datadog was serializing objects passed to the AsyncEventReporter which caused a send failure

```csharp
[GuaranteedRate.Sextant "17.2.2.12"]
```

## v17.2.2.11 / 2017 Sep 05
* **Update** - Update Logger level naming convention to suffix `_LEVEL` so as to make IDE typeahead more pleasant
 
```csharp
[GuaranteedRate.Sextant "17.2.2.11"]
```

## v17.2.2.10 / 2017 Sep 05
* **Fix** - Make loggly log reporter run last so we can debug an intermittent loggly issue.

```csharp
[GuaranteedRate.Sextant "17.2.2.10"]
```

## v17.2.2.9 / 2017 Sep 05
* **Fix** - Bump version to assist in debugging errors.

```csharp
[GuaranteedRate.Sextant "17.2.2.9"]
```

## v17.2.2.8 / 2017 Aug 28
* **Add** - Added support for automatic configuration of the `ConsoleLogAppender` when utilizing `Logger.Setup()`
 
```csharp
[GuaranteedRate.Sextant "17.2.2.8"]
```

## v17.2.2.7 / 2017 Aug 28
* **Fix** - Fix regression that prevented loggers from filtering logs based on configured log level tolerance
 
```csharp
[GuaranteedRate.Sextant "17.2.2.7"]
```

## v17.2.2.6 / 2017 Aug 28
* **Fix** - Regression in Loggly support that was preventing logs from being searchable
* **Add** - Added ability to add runtime tags to `Logger` via `Logger.AddTag()`
* **Add** - Support for tagging in Elassticsearch
 
```csharp
[GuaranteedRate.Sextant "17.2.2.6"]
```

## v17.2.2.5 / 2017 Aug 22
* **Add** - Added ability to add global runtime tags for `StatsDMetrics`

```csharp
[GuaranteedRate.Sextant "17.2.2.5"]
```

## v17.2.2.4 / 2017 Aug 22
* **Add** - Added ability to set global tags for `StatsDMetrics`

```csharp
[GuaranteedRate.Sextant "17.2.2.4"]
```

## v17.2.2.3 / 2017 Aug 21
* **Update** - Added `Loggers.Setup()` to allow for the same configuration flow as seen in `StatsDMetrics`

```chsarp
[GuaranteedRate.Sextant "17.2.2.3"]
```

## v17.2.2.2 / 2017 Aug 04
* **Update** - Metrics.NET was released to stable, updating the nuget packages to reflect the change
```csharp
[GuaranteedRate.Sextant "17.2.2.2"]
```

## v17.2.2.1 / 2017 Jul 27
* **Fix** - Bugfix to throw a meaningful exception if an `AsyncEventReporter` has been created without a base url
* **Fix** - Bugfix to correctly look for `LogglyLogAppender.Tags` in configuration files for the `LogglyLogAppender`
* **Fix** - Correct a regression that was preventing `SecureAsyncEventReporter` from settings it's authorization header
 
```csharp
[GuaranteedRate.Sextant "17.2.2.1"]
```

## v17.2.2.0 / 2017 Jul 26
> This release intruduces the concept of StatsD driven metrics tracking.  We are leveraging [Metrics.NET](https://github.com/Recognos/Metrics.NET)
to backbone the effort of transposing metrics into StatsD compliant metrics for consumption by our third party reporters.  
We have elected to supoort Graphite with the `GraphiteReporter` from [Metrics.NET.Graphite](https://github.com/Recognos/Metrics.NET.Graphite) and
Datadog with the `DatadogReporter` from  [Metrics.NET.Datadog](https://github.com/danzel/Metrics.NET.Datadog)
* **Add** - `SimpleMetrics` and `StatsDMetrics` as available Metrics trackers
* **Update** - Sextant has been rolled up to .NET 4.5.2 to utilize Metrics.NET 

```csharp
[GuaranteedRate.Sextant "17.2.2.0"]
```

## v17.2.1.0 / 2017 Jul 13
> This release intruduces a new approach to logging and metrics tracking.  Instead of instantiating a Loggly logger you can now use the all 
purpose `Logger` object and attach an appender (such as `LogglyAppender` and `ElasticsearchLogAppender`) to log system data.  Following suit
with this, we are now exposing `Metrics`.  This tool will aloow for adding `DatadogReporter` and `GraphiteReporter` reporters for tracking
system metrics. 
* **Fix** - Bugfix for IniConfig.GetValue(string key, bool defaultValue) - previously, this method never actually pulled values from the config
* **Add** - `LogglyAppender` and `ElasticsearchLogAppender` as starter appenders for Logger
* **Add** - `DatadogReporter` and `GraphiteReporter` as starter appenders for Metrics
* **Update** - `AsyncEventReporter.PostEvent()` is now virtual and can be overriden by derived classes

```csharp
[GuaranteedRate.Sextant "17.2.1.0"]
```

## v17.2.0.6 / 2017 Apr 27
* **Fix** - for Json configs of <T>

```csharp
[GuaranteedRate.Sextant "17.2.1.0"]
```

## v17.2.0.5 / 2017 Apr 21
* **Add** - Have SessionUtils support Loan OpenLoan(Session session, Guid guid)

```csharp
[GuaranteedRate.Sextant "17.2.0.5"]
```

## v17.2.0.4 / 2017 Apr 21
* **Update** - Encompass expects string interpretations of Guids to be wrapped in braces.  Safe checking for this as Guid.ToString() will emit a guid that does not follow this standard.

```csharp
[GuaranteedRate.Sextant "17.2.0.4"]
```

## v17.2.0.3 / 2017 Apr 17
* **Update** - This release upgrades Newtonsoft to 10.0.2 which is needed by Encompass 17.2.0.2

```csharp
[GuaranteedRate.Sextant "17.2.0.3"]
```

## v17.2.0.2 / 2017 Apr 17
* **Update** - This release allows for support of Encompass 17.2.0.2

```csharp
[GuaranteedRate.Sextant "17.2.0.2"]
```

## v17.1.0.5 / 2017 Mar 20
* **Fix** - This release corrects a regression in LoanDataUtils that allowed null values to be set to properties instead of leaving unset properties as empty strings.

```csharp
[GuaranteedRate.Sextant "17.1.0.5"]
```

## v17.1.0.4 / 2017 Mar 16
* **Add** - This release extends IJsonEncompassConfig to support T GetValue<T>(string key, T defaultValue = default(T))

```csharp
[GuaranteedRate.Sextant "17.1.0.4"]
```

## v17.1.0.3 / 2017 Mar 07
* **Update** - Extending JsonEncompassConfig to implement an IJsonEncompassConfig

## v17.1.0.2 / 2017 Mar 07
> This release updates the IniConfig to be able to accept values that contain an equal sign

## v17.1.0.1 / 2017 Feb 23
> This release forces trimming on field ids to prevent Encompass "Field ID not found" errors 

## v17.1.0.0 / 2017 Feb 08
> This release updates the Encompass SDK to version 17.1 and includes instructions 
> for building your own NuGet package.

* **Update** - Encompass SDK to 17.1.
* **Add** - Encompass Nuget package nuspec and instructions.

```csharp
[GuaranteedRate.Sextant "17.1.0.0"]
```

## v16.2.7.1 / 2017 Jan 27

> This bugfix release fixes `AsyncEventReporter`, it considered some http 
> status codes as failures, which resulted in extra retries

* **Fix** - `AsyncEventReporter` now considers 201 & 204 successful responses

```csharp
[GuaranteedRate.Sextant "16.2.7.1"]
```

## v16.2.7.0 / 2017 Jan 20

> This release adds functionality to lookup a loan guid by loan number.

* **Add** - Expose a function to return the loan guid for a given loan number.

```csharp
[GuaranteedRate.Sextant "16.2.7.0"]
```

## v16.2.6.0 / 2017 Jan 17

> This release adds various pieces of Funding and CD Fees to the set of data
> extracted by `LoanDataUtils`.  It appears that some or all of the data is 
> duplicated elsewhere, but this addition makes the data more accessable

* **Add** - LoanFee data to the set of data extracted in `LoanDataUtils`

```csharp
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

```csharp
[GuaranteedRate.Sextant "16.2.5.3"]

```

## v16.2.5.2 / 2016 Sep 27
> Fix typo in Loggly.cs

* **Update**  - Loggly.cs  

```csharp
[GuaranteedRate.Sextant "16.2.5.2"]

```

## v16.2.5.1 / 2016 Sep 27
> Add loanId and loanGuid to the exception.

* **Update**  - LoanDataUtils 

```csharp
[GuaranteedRate.Sextant "16.2.5.1"]

```
## v16.2.5.0 / 2016 Sep 20
> Added Init(string) to the config.
> Supports loading from known strings.  Useful for tests.

* **Update**  - JsonEncompassConfig
* **Update**  - IniConfig
* **Update**  - IEncompassConfig

```csharp
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

```csharp
[GuaranteedRate.Sextant "16.2.4.0"]
```
## v16.2.3.0 / 2016 Sep 9
> Added the JsonEncompassConfig to support using JSON for configuration files
> and added support for Loggly.Fatal errors for when things go really wrong.

* **Add**  - JsonEncompassConfig
* **Add**  - Loggly.Fatal

```csharp
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

```csharp
[GuaranteedRate.Sextant "16.2.2.0"]
```

## v16.2.1.0 / 2016 Aug 9

> This version removes the dependency on the EncompassSDK.
> We did this because the SDK must be installed on the target machine in all 
> cases, and having a nuget dependency only intruduces potential dependency 
> conflicts

* **Update** - Removed the EncompassSDK nuget package, and added hardcoded
references to the installed EncompassSDK

```csharp
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

```csharp
[GuaranteedRate.Sextant "16.2.0.0"]
```

## v1.14.0 / 2016 June 10

> Merge screwup - lots of code in thsi release that was never merged to Master.
> This release adds `SecureAsyncEventReporter` which allows posting data to an
> endpoint that requires an `Authorization` token.

* **Add** - `SecureAsyncEventReporter` which will add an `Authorization` token
to the header of the events being posted.
* **Add** - Methods for extracting user/licensing info in `UserUtils`

```csharp
[GuaranteedRate.Sextant "1.14.0"]
```

## v1.13.3 / 2016 Jun 09

> Added new interface to support IoC containers, minor bugfix, added AddMeter functionality.

* **Add** - Added AddMeter to datadog reporter
* **Fix** - Correct spelling in datadog reporter.  `guage` should be `gauge`.  Add additional method to support old spelling.  Mark old spelling "obsolete."

```csharp
[GuaranteedRate.Sextant "1.13.3"]
```

## v1.13.2 / 2016 May 17

> Bug in `LoanDataUtils` - `PostClosingConditions` were being cast as `UnderwritingConditions`

* **Fix** - Fixed invalid object casting in `LoanDataUtils`

```csharp
[GuaranteedRate.Sextant "1.13.2"]
```

## v1.13.1 / 2016 May 2

> Switched back to `EncompassSDK.Complete`, `EncompassSDK.Standard` is
> insufficient for SDK apps.

* **Fix** - Switched back from EncompassSDK.Standard to EncompassSDK.Complete.  
Standard isn't sufficient for standalone apps.

```csharp
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

```csharp
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

```csharp
[GuaranteedRate.Sextant "1.12.2"]
```

## v1.12.1 / 2016 March 21

> Updating to Encompass SDK 15.2.  No other changes.

```csharp
[GuaranteedRate.Sextant "1.12.1"]
```

## v1.12.0 / 2016 March 17

> Adding the ability to collect active directory and machine information
> Adding AD and machine information to ExtractEverything method data return

```csharp
[GuaranteedRate.Sextant "1.12.0"]
```

## v1.11.3 / 2016 Feb 23

> Fixing bug in `AsyncEventReporter` - the function `ExtraSetup` wasn't marked
> as override

* **Fix** - Java dev makes C# mistake...

```csharp
[GuaranteedRate.Sextant "1.11.3"]
```

## v1.11.2 / 2016 Feb 23

> Fixing bug in `AsyncEventReporter` - the function `ExtraSetup` wasn't being
> called.

* **Fix** - `AsyncEventReporter` now calls `ExtraSetup` when setting up an event

```csharp
[GuaranteedRate.Sextant "1.11.2"]
```

## v1.11.1 / 2016 Feb 2

> Updated logging in `LoanDataUtils` to include the `loanNumber` where the 
> exception occured

* **Update** - `LoanDataUtils` logging to include the loan number

```csharp
[GuaranteedRate.Sextant "1.11.1"]
```

## v1.11.0 / 2016 Feb 2

> Added the ability to remove fields from `FieldUtils` so that specific fields
> can be ignored without forcing the user to add all the others in the 
> collection.

* **Update** - `FieldUtils` now supports removing specific fields

```csharp
[GuaranteedRate.Sextant "1.11.0"]
```

## v1.10.6 / 2016 Jan 27

> Cleanup from the date formatting madness.

* **Update** - `LoanDataUtils` now has a configurable datetime formatter

```csharp
[GuaranteedRate.Sextant "1.10.6"]
```

## v1.10.5 / 2016 Jan 27

> Trying date-time formatter again

```csharp
[GuaranteedRate.Sextant "1.10.5"]
```

## v1.10.4 / 2016 Jan 27

> Restoring Loan.LastModified code from master

```csharp
[GuaranteedRate.Sextant "1.10.4"]
```

## v1.10.3 / 2016 Jan 27

> Fix bug in `AysncEventReporter` where it would retry events that were 
> `ACCEPTED` instead of `OK`

* **FIX** - `AysncEventReporter` now has a set of `success` http response codes:
HttpStatusCode.OK, HttpStatusCode.Accepted, HttpStatusCode.Continue

```csharp
[GuaranteedRate.Sextant "1.10.3"]
```

## v1.10.2 / 2016 Jan 27

> Non-release debug version for tracking the LastModified problem

```csharp
[GuaranteedRate.Sextant "1.10.2"]
```

## v1.10.1 / 2016 Jan 26

> This release adds defensive code around getting the value of loan.LastModified.
> The value returned is not always valid, so the defensive code will do 
> polling and attempt to correct.

* **Fix** - New logic for getting last modified in 
`LoanDataUtils.GetBestGuessLastModified(Loan loan)`

```csharp
[GuaranteedRate.Sextant "1.10.1"]
```

## v1.10.0 / 2016 Jan 25

> This release corrects the shutdown mechanics of `AsyncEventReporter` so that
> callers are able to shutdown without losing data that is on the queue.

* **Fix** - `AsyncEventReporter.Shutdown()` to shutdown cleanly.  A side effect
of this change is that this method now *BLOCKS* until the reporter has shutdown

```csharp
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

## v1.4.0 / 2015 Oct 8

> Improvements to Borrower-Pair collection

* **Update** - FieldUtils.BORROWER_PAIR_FIELDS - Add field "1268", Co-Borrower email

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
* **Added** - `LoanDataUtils.ExtractMilestones` which returns a list of milestone data.
* **Added** - `LoanDataUtils.ExtractEverything` which runs all the extraction methods and returns a dictonary of the collections.

## v1.0.1 / 2015 Aug 28
* **Update** - Update to FieldUtils to make it possible to collect a subset of fields instead of always getting all fields.
* **Update** - Revised the readme to be more descriptive

## v1.0.0 / 2015 Aug 27

> Initial release.
> This code is being copied and sanitized from an existing internal repo.
> Losing the history is intentional.
