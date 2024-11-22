# NLogWebAPI
WebAPI Send Logs to Azure Log Analytics

### NLog Configuration (`nlog.config`)

This configuration file is used to set up NLog targets and rules for logging in an application. The logging is directed to multiple outputs, including Azure Log Analytics, the console, and local log files. Below is a breakdown of the configuration:

## nlog.config
```
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	  throwConfigExceptions="true"
	  >

	<!--//======================================= Register Custom Target ==============================================//-->
	<extensions>
		<add assembly="NLogWebAPI" />
	</extensions>

	<!--//======================================= Internal logging for NLog debugging =================================//-->	 
	<internalLogFile>${basedir}/Logs/nlog-internal.log</internalLogFile>
	<internalLogLevel>Debug</internalLogLevel>
	<targets>
		<!--//................................... Target for File logging for internal logs .......................... -->

		<!--
		<target name="internalLogTarget" xsi:type="File"
                fileName="${basedir}/Logs/nlog-internal-${shortdate}.log"
				layout= "[${longdate}] [${level:uppercase=true}] ${message} ${exception:format=toString,stackTrace}"
                archiveEvery="Day"
                archiveNumbering="Date"
                maxArchiveFiles="5" />
		-->

		<target name="internalLogTarget" xsi:type="File"
                fileName="${basedir}/Logs/nlog-internal.log"
				layout= "[${longdate}] [${level:uppercase=true}] ${message} ${exception:format=toString,stackTrace}"
				archiveAboveSize="51200" 
                archiveNumbering="Sequence"
                archiveFileName="${basedir}/Logs/nlog-internal.{#}.log"
                maxArchiveFiles="5" />
	 
	</targets>
	
	<!--//======================================= Redirect internal logs to file target ===============================//-->
	<rules>
		<logger name="*" minlevel="Debug" writeTo="internalLogTarget" />
	</rules>


	<!--//======================================= Targets for application logging =====================================//-->
	<targets>
		<!--//................................... Target for Azure Log Analytics ......................................//-->
		<target name="logAnalytics" xsi:type="LogAnalytics"
                customerId="{workspace ID}"
                sharedKey="{Primary key}"
                logType="tni_nlog_app" />

		<!--//................................... Target for Console ..................................................//-->
		<target name="logConsole1" xsi:type="Console"		
				layout="${longdate} | ${level} | ${message} | ${exception:format=toString} | ${all-event-properties}" 
				/>

		<target name="logConsole2" xsi:type="Console"
				layout= "[${longdate}] [${level:uppercase=true}] ${message} ${exception:format=toString,stackTrace}"
				/>
		 

		<!--//................................... Target for File logging with layout .................................//-->
		<!--<target name="file" xsi:type="File"
                fileName="logs/logfile.txt"
                layout="${longdate} | ${level:uppercase=true} | ${message} ${exception:format=toString,stackTrace}" />
				-->

		<target name="logfile" xsi:type="File"
				fileName="${basedir}/Logs/tni_nlog_app_${shortdate}.log"
				layout="[${longdate}] [${uppercase:${level}}] ${message} ${exception:format=tostring}"
				archiveAboveSize="51200"
				archiveNumbering="Sequence"
				archiveFileName="${basedir}/Logs/tni_nlog_app_${shortdate}.{#}.log"
				maxArchiveFiles="5" />
 

	</targets>

	<!--//======================================= Rules for logging ===================================================//-->
	<rules>
		<!-- Log all to Azure Log Analytics and Console -->
		 <logger name="*" minlevel="Debug" writeTo="logAnalytics,logConsole2,logfile" />  
		
	</rules>

</nlog>


```

#### Key Features:

1. **Custom Log Analytics Target:**
   - Logs are sent to Azure Log Analytics using a custom NLog target (`LogAnalyticsTarget`).
   - The target is configured with the following properties:
     - **CustomerId**: Azure Log Analytics workspace ID (`{workspace ID}`).
     - **SharedKey**: Authentication key for Log Analytics (`{Primary key}`).
     - **LogType**: Custom log type name (`tni_nlog_app`).

2. **Console Output:**
   - Logs are printed to the console with two different layouts:
     - `logConsole1` outputs logs with properties such as level, message, and exception details.
     - `logConsole2` uses a simplified layout with timestamp, level, message, and stack trace for exceptions.

3. **File Output:**
   - Logs are written to a log file with the following setup:
     - **File Name**: Logs are saved under the directory `Logs/`, with filenames including the short date (`tni_nlog_app_${shortdate}.log`).
     - **Archiving**: Logs are archived when the file size exceeds 50 MB, with a maximum of 5 archived log files.

4. **Internal Logging:**
   - NLog’s internal logs are directed to a file (`nlog-internal.log`) for debugging purposes.
   - The internal log target will archive logs if the file size exceeds 50 MB.

5. **Logging Rules:**
   - All loggers (`logger name="*"`) with a minimum level of `Debug` are configured to log to the following targets:
     - Azure Log Analytics (`logAnalytics`).
     - Console (`logConsole2`).
     - Log file (`logfile`).

#### Summary:
- This configuration allows logging to multiple destinations, including Azure Log Analytics, local files, and the console.
- Internal logging for NLog itself is enabled for debugging purposes.
- Logs are archived when file sizes exceed a specified threshold to manage log file growth.

### LogAnalyticsTarget Class

##
#### Date Created: `2024-11-20`
#### Author: `Nuchit Atjanawat`

##
The `LogAnalyticsTarget` class is a custom NLog target that sends log entries to Azure Log Analytics. This class is designed to be used within an NLog configuration, allowing logs to be forwarded to Azure for analysis and monitoring.

#### Properties:

- **CustomerId**: The `Azure Log Analytics workspace customer ID` used for authentication.
- **SharedKey**: The shared key or `Primary key` used to authenticate log entries to the Azure Log Analytics service.
- **LogType**: The `name of the log type` to be used when sending log entries to Log Analytics.

#### Methods:
- **InitializeTarget()**: Initializes the LogAnalytics client with the provided `CustomerId` and `SharedKey`.
- **Write(LogEventInfo logEvent)**: Overrides NLog's `Write` method to send log entries to Azure Log Analytics. If the log event contains `LogEntities`, it serializes and sends the entries to the configured Log Analytics workspace.

## Install Required NuGet Packages
To use TniLoggerService, ensure the following NuGet packages are installed:
- **LogAnalytics.Client**
```
dotnet add package LogAnalytics.Client
```

#### Dependencies:
- **LogAnalytics.Client**: A client that communicates with Azure Log Analytics. `LogEntity`: A custom class representing the structure of log entries to be sent to Log Analytics.

#### Usage:
To use the `LogAnalyticsTarget`, configure it in your NLog configuration file, providing the necessary `CustomerId`, `SharedKey`, and `LogType`. Log entries containing `LogEntities` will be sent asynchronously to Azure Log Analytics.



# TniLoggerService Class

#### Date Created: `2024-11-20`
#### Author: `Nuchit Atjanawat`

##
The `TniLoggerService` class provides a centralized logging solution for the application, combining the flexibility of `Microsoft.Extensions.Logging` and the advanced features of `NLog`. This class is designed to handle various logging needs, including structured logs, detailed error reporting, and integration with external systems like Azure Log Analytics.

## Key Features
1. **Support for Multiple Log Levels**:
   - Includes methods to log at different levels: `Info`, `Debug`, `Warn`, `Trace`, and `Error`.
   - Ensures traceability and granularity of log messages.

2. **Integration with NLog**:
   - Leverages NLog for enhanced logging capabilities like custom layouts and external configurations.
   - Reads layout configurations from the `NLog.config` file and applies them dynamically.

3. **Azure Log Integration**:
   - Supports sending batches of log entries (`LogEntity`) to Azure Log Analytics.
   - Provides a reliable mechanism to monitor logs in cloud environments.

4. **Custom Log Entities**:
   - Creates structured log entries (`LogEntity`) containing detailed metadata:
     - **Message**: The formatted log message.
     - **Level**: The log level (e.g., Info, Debug, Error).
     - **Timestamp**: UTC time of the log entry.
     - **Details**: Additional data (e.g., stack trace, custom properties).

5. **Error Handling**:
   - Logs errors with exception details, including stack traces and additional metadata, ensuring proper debugging information.

6. **Microsoft LogLevel Conversion**:
   - Converts `Microsoft.Extensions.Logging` log levels into `NLog` log levels using an extension method for compatibility.

## How It Works
1. **Initialization**:
   - Instantiates the `Microsoft.Extensions.Logging` logger and the `NLog` logger.
   - Loads the `NLog.config` layout or applies a default layout if none is found.

2. **Logging**:
   - Logs messages using the appropriate log level method:   
     - `LogInfo`
     - `LogDebug`
     - `LogWarn`
     - `LogTrace`
     - `LogError`
   - Formats logs and stores them both locally and in external systems (e.g., Azure).

3. **Azure Log Sending**:
   - Gathers a list of `LogEntity` objects and sends them to Azure Log Analytics via the `SendLogToAzure` method.

4. **Structured Logging**:
   - Formats and serializes detailed log messages with optional details for consistent reporting.

## Example Usage
```csharp
var loggerService = new TniLoggerService(logger);

// Log informational messages
loggerService.LogInfo("Application started");

// Log detailed error messages with exception
try
{
    // Some code that may throw an exception
}
catch (Exception ex)
{
    loggerService.LogError("An error occurred", ex);
}

// Send logs to Azure
var logEntities = new List<LogEntity>
{
    new LogEntity { Message = "Log 1", Level = "Info", Timestamp = DateTime.UtcNow },
    new LogEntity { Message = "Log 2", Level = "Error", Timestamp = DateTime.UtcNow }
};
loggerService.SendLogToAzure(logEntities);
```

## Install Required NuGet Packages
To use TniLoggerService, ensure the following NuGet packages are installed:
1. **LogAnalytics.Client**
```
dotnet add package LogAnalytics.Client
```
2. **NLog.Web.AspNetCore**
```
dotnet add package NLog.Web.AspNetCore
```


## Dependencies
To use TniLoggerService, ensure the following NuGet packages are installed:

1. **LogAnalytics.Client**
2. **NLog.Web.AspNetCore**

## Conclusion
The TniLoggerService class simplifies logging by providing a unified interface for local and cloud-based logging. It is highly extensible and ensures that all logs are structured, consistent, and reliable, making it a valuable tool for any modern application.



