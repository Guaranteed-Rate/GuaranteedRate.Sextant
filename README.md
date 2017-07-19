**[Updates](#updates)** | **[Config](#config)** | **[Logging](#logging)** | **[Metrics Tracking](#metrics-tracking)** | **[ILMerge](#ilmerge)** | **[Testing Programs](#testing-programs)** | **[Developer Notes](#developer-notes)**

# GuaranteedRate.Sextant

Sextant is a collection of utilities for working with Ellie Mae's Encompass.
Most importantly, this library works with **BOTH** Encompass Plugins and standalone SDK Apps.

This allows developers to bake configuration, logging and performance into plugins, which will reduce development time, decrease bugs, make deployments easier and improve user experience.

The primary goal is to extend plugin functionality in a few key areas:
* A common and consistant configuration system using common data objects
* Logging outside of Encompass (via Loggly or some other mechanism) so that Logs are easy to access, searchable, and do not add load onto the Encompass server.
* Metrics (via Datadog or some other mechanism) which allows tracking of events, latency, usage, etc without adding any load onto the Encompass server.

While this code is general purpose and should work for everyone, the adapters for Datadog (http://datadog.com) and Loggly (http://loggly.com) will only be useful if your organization uses these tools.  Both companies offer a free tier which are sufficient for small organizations or for demo purposes.

## Updates
For a comprehensive list of version updates.  Please consult our [Changlog](CHANGELOG.md)

## Config

`IEncompassConfig` is an interface for using an Encompass CustomDataObject as a config file.

### IniConfig
> This has been deprecated.  Please consult the [JsonEncompassConfig](#jsonencompassconfig)

**IniConfig** is a simple implementation that treats the config file like a classic INI file. 
It is important to note that the `IniConfig` will lowercase all values handed to it, so it is not an ideal configuration store for case sensitive configurations such as database passwords.  

```
[Key]=[Value]\n
# Lines starting with # are considered comments
# (Blank lines are ignored)
```

Sample INI file
```
# Datadog configurations
DatadogReporter.ApiKey 0123456789012345678901234567890

# Loggly configurations
LogglyLogAppender.Error.Enabled true
LogglyLogAppender.Warn.Enabled false
```

### JsonEncompassConfig
**JsonEncompassConfig** is a simple implementation that uses a json config file.  

Sample Json file
```javascript
{
  "ElasticsearchLogAppender": {
    "Url": "https://my.elasticsearch.com",
    "Debug": { "Enabled": true },
    "Info": { "Enabled": true },
    "Warn": { "Enabled": true },
    "Error": { "Enabled": false },
    "Fatal": { "Enabled": true },
	"Tags": "dev,project1",
	"QueueSize": 1000,
	"RetryLimit": 3
  }
}
```

## Logging

> As Encompass uses `log4net` under the hood we are not able to leverage `log4net` in our plugins.  
Born out of necessity, we have created the GuaranteedRate.Logging library to support alternative logging means

### Logger

**Logger** is a static holding object that can contain any number of `ILogAppender` objects.  Once a appender is registered with Logger it will be called each time `Logger` is invoked in your Application

Sample usage
```csharp
var config = new JsonEncompassConfig();
config.Init(System.IO.File.ReadAllText("SextantConfigTest.json"));
                
var console = new ConsoleLogAppender(config);

var loggly = new LogglyLogAppender(config);
var elasticSearch = new ElasticsearchLogAppender(config);

Logger.AddAppender(console);
Logger.AddAppender(loggly);
Logger.AddAppender(elasticSearch);

Logger.Debug("SextantTestRig", "Test debug message");
Logger.Info("SextantTestRig", "Test info message");
Logger.Warn("SextantTestRig", "Test warn message");
Logger.Error("SextantTestRig", "Test error message");
Logger.Fatal("SextantTestRig", "Test fatal message");

```

- An example usage of `Logger` in action can be found in [LoggingTestRig](LoggingTestRig/Program.cs)

### Loggly

**LogglyLogAppender** is an asynchronous web client that sends data directly to Loggly's RESTful interface. 
The `LogglyLogAppender` is built on top of [**AsyncEventReporter**](#asynceventreporter) and will run in a separate thread 
freeing your application main thread to continue with other work.  

- An example usage of the `LogglyLogAppender` in isolation can be found in [GuaranteedRate.Sextant.Integration.Tests](GuaranteedRate.Sextant.Integration.Tests/Logging/Loggly/LogglyLogAppenderIntegrationTests.cs)
- An example usage of the `LogglyLogAppender` as a log appender attached to Logger can be found in [LoggingTestRig](LoggingTestRig/Program.cs)

Example:

```csharp
Logger.Error(this.GetType().Name.ToString(), "This is my error");
IDictionary<string, string> fields = new Dictionary<string, string>();
fields.Add("foo", "bar");
fields.Add("foo2", "bar2");
Logger.Info(this.GetType().Name.ToString(), fields);
```

### Elastic Search

**ElasticsearchLogAppender** is an asynchronous web client that sends data directly to Loggly's RESTful interface. 
The `ElasticsearchLogAppender` is built on top of [**AsyncEventReporter**](#asynceventreporter) and will run in a separate thread 
freeing your application main thread to continue with other work.  

- An example usage of the `ElasticsearchLogAppender` in isolation can be found in [GuaranteedRate.Sextant.Integration.Tests](GuaranteedRate.Sextant.Integration.Tests/Logging/Elasticsearch/ElasticsearchLogAppenderIntegrationTests.cs)
- An example usage of the `ElasticsearchLogAppender` as a log appender attached to Logger can be found in [LoggingTestRig](LoggingTestRig/Program.cs)

## Metrics tracking

### Metrics

**Metrics** is a static holding object that can contain any number of `IReporter` objects.  Once a reporter is registered with `Metrics` it will be called each time `Metrics` is invoked in your Application

Sample usage
```csharp
var config = new JsonEncompassConfig();
config.Init(System.IO.File.ReadAllText("SextantConfigTest.json"));
                
var datadog = new DatadogReporter(config);
var graphite = new GraphiteReporter(config);

Metrics.AddReporter(datadog);
Metrics.AddReporter(graphite);

Metrics.AddGauge("SextantTestRig.Gauge", 10);
Metrics.AddCounter("SextantTestRig.Counter", 10);
Metrics.AddMeter("SextantTestRig.Meter", 10);

```

- An example usage of `Metrics` in action can be found in [LoggingTestRig](LoggingTestRig/Program.cs)


### Datadog

**DatadogReporter** is an asynchronous web client that sends series data directly to datadog's RESTful interface. 
The `DatadogReporter` is built on top of [**AsyncEventReporter**](#asynceventreporter) and will run in a separate thread 
freeing your application main thread to continue with other work.  

- An example usage of the `DatadogReporter` in isolation can be found in [GuaranteedRate.Sextant.Integration.Tests](GuaranteedRate.Sextant.Integration.Tests/Metrics/Datadog/DatadogReporterIntegrationTests.cs)
- An example usage of the `DatadogReporter` as a reporter attached to Metrics can be found in [LoggingTestRig](LoggingTestRig/Program.cs)

### Graphite

**GraphiteReporter** is an asynchronous web client that sends series data directly to datadog's RESTful interface. 
The `GraphiteReporter` is built on top of [**AsyncEventReporter**](#asynceventreporter) and will run in a separate thread 
freeing your application main thread to continue with other work.  

- An example usage of the `GraphiteReporter` in isolation can be found in [GuaranteedRate.Sextant.Integration.Tests](GuaranteedRate.Sextant.Integration.Tests/Metrics/Graphite/GraphiteReporterIntegrationTests.cs)
- An example usage of the `GraphiteReporter` as a reporter attached to Metrics can be found in [LoggingTestRig](LoggingTestRig/Program.cs)

## AsyncEventReporter

This is a simple (copied from msdn) mutli-threaded queue that will send POSTs to a service asynchronously.
The calling thread adds to a Queue, and internally a Task thread will read the queue and send POSTs.

Note on queue sizes: If/when the queue is full, the writing thread will block until there is room on the queue.  Since the writing thread comes from Encompass, Encompass will seem to hang when this occurs.  Set up your queue sizes to be larger than you expect to need.

When run as an Encompass plugin, a moderate machine will drain the queue at about 600 events/minute.

## ILMerge

To use this library in an Encompass plugin, you will need to use a free Microsoft util called ILMerge to recompile the plugin into your dll.

Download ILmerge: http://www.microsoft.com/en-us/download/details.aspx?id=17630

Create a post-build step like this:
<p  align="center">
  <img  src="doc/img/ilMerge-postBuild.png" border="0" />
</p>

```csharp

set ilMergePath=C:\Program Files (x86)\Microsoft\ILMerge
set outPath=Combined\
mkdir %outPath%
 
<#
This line will combine the project DLL with any DLLs that you specify and copy it to a new 'Combined' directory.
You have to explicitly state the DLLs you want to merge because the Encompass DLLs will also be in the directory and those should not be included.
In this example we are merging Newtonsoft.Json and ReportingUtils
#>
"%iLMergePath%\ILMerge.exe" /out:%outPath%$(TargetName).dll $(TargetFileName) Newtonsoft.Json.dll GuaranteedRate.Sextant.dll /wildcards  /targetplatform:v4
 
 
<# This part is unnecessary, it will deploy your plugin to your LOCAL Encompass plugin dir so that your local-deploy will auto-deploy locally #>
 
if "$(ConfigurationName)" == "Debug" (
set targetFileWrapped= $(ProjectDir)$(OutDir)%outPath%$(TargetName).dll
xcopy %targetFileWrapped% "%USERPROFILE%\AppData\LocalLow\Apps\Ellie Mae\xIHR5EqGa7zPnRG0YpD5z4TPAB0=\EncompassData\Settings\Cache\33af7d98-3c15-497e-937e-b83215be32bc\Plugins" /f/y)
 
```

You will also need to add the following additional packages to be included in your plugin:

* json.net (newtonsoft)

## Testing Programs

### FieldUtils

This is an executable utility program that will generate a file `FieldsAndDescriptions.csv` which contains all fieldIds and their definition on the Encompass server. 

### LoanUtils

This is an executable utility program that will open a loan and extract the data in json format.
The results will be written to the command line.

Useful for testing and debugging. 

### ConfigUtil

Simple test/example program for using IEncompassConfig.
Loads a config file from the Encompass Server and writes the key/value pairs to the console.

### ReportingUtils

Simple test/example program for using `Reporting.cs`.
Queries Encompass for loans last modified between a start and end date (half open) and returns a list of GUIDs.

### ReportingUtils.Functional

This is a simple test harness for the Datadog and Loggly posters.
Because this test sends data to Datadog and Loggly it is not written as a unit test.

### UserUtils

Simple example program for usin 'UserUtils.cs'.
Creates a session and prints all users, all "active" users (enabled and unlocked accounts), and all users associated within the standard "My Pipeline" working folder.

### Util.IndexFields

This is a util project to help find the boundary index for MultiValue fields that use an index.
Knowing the boundary index for multivalue fields allows faster and safer itteration.

It will show you MultiValue Index fields without known boundaries, as well as the values of the known indexes.

## Developer Notes

### Legacy Security Policy Setup

Steps below are recommended by EllieMae, but we found them not required for the application to work
* Enable legacy security policy
* * For ConsoleApplication/WindowsService Add this to app.config:
```
<configuration>
<runtime>
<NetFx40_LegacySecurityPolicy enabled="true"/>
</runtime>
</configuration>
```
* * For Web Application add this to web.config:
```
<configuration>
<system.web>
<trust level="Full" legacyCasModel="true"/>
</system.web>
</configuration>
```
* Initialize encompass SDK by calling:
``` new EllieMae.Encompass.Runtime.RuntimeServices().Initialize(); ```
at the begining of the your application execution.

### Fields controlled by dropdowns - default is often null

There are many fields in Encompass that are controlled by dropdowns.
In many cases, if the value has never been changed from the default, the actual data value from the SDK will be null.

For example Field "1393" (Trans Details Current Loan Status):
<p  align="center">
  <img  src="doc/img/Field1393.png" border="0" />
</p>

The dropdown has a default value of "Active Loan", but if the field has never been touched, then the value will be empty.

ie you can see the drop down showing "Active Loan", but in the SDK will show:
```csharp
loan.Fields["1393"].Value == "";
```
