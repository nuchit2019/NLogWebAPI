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

