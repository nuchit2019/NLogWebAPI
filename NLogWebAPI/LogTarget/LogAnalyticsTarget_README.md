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

