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
