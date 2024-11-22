// ======================================================================================//
// Date Created: 2024-11-20
// Author: Nuchit Atjanawat
// ======================================================================================//
//....................................................................................... Install Required NuGet Packages
//....................................................................................... 1. dotnet add package LogAnalytics.Client
//....................................................................................... 2. dotnet add package NLog.Web.AspNetCore


using NLog;
using NLogWebAPI.Models;
using Newtonsoft.Json;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using NLog.Layouts;

namespace NLogWebAPI.Services
{
    public interface ITniLoggerService
    {
        void LogInfo(string message, object details = null);
        void LogDebug(string message, object details = null);
        void LogWarn(string message, object details = null);
        void LogTrace(string message, object details = null);
        void LogError(string message, Exception ex, object additionalDetails = null);
        void SendLogToAzure(List<LogEntity> logEntities); 
        LogEntity CreateLogEntity(LogEventInfo logEvent);
    }

    public class TniLoggerService : ITniLoggerService
    {
        private readonly ILogger<TniLoggerService> _logger; //.............................. Microsoft.Extensions.Logging instance.
        private readonly NLog.Logger _nLogger; //........................................ NLog logger instance.
        private readonly Layout _layout; //.............................................. Layout instance.

        public TniLoggerService(ILogger<TniLoggerService> logger)
        {
            _logger = logger; //......................................................... Assign Microsoft.Extensions logger.
            _nLogger = LogManager.GetCurrentClassLogger(); //............................ Initialize NLog logger for the current class.

            //........................................................................... Load NLog config...
            if (LogManager.Configuration == null)
            {
                throw new InvalidOperationException("NLog configuration not load...");
            }
            //........................................................................... Find the target and get its layout
            var target = LogManager.Configuration.FindTargetByName<NLog.Targets.TargetWithLayout>("logfile");
            if (target == null)
            {
                //....................................................................... Use default layout if target is not found
                _layout = new SimpleLayout("[${longdate}] [${uppercase:${level}}] ${message} ${exception:format=tostring}");
            }
            else
            {
                _layout = target.Layout;
            }

        }

        public void LogInfo(string message, object details = null) => LogWithLevel(LogLevel.Information, message, details);
        public void LogDebug(string message, object details = null) => LogWithLevel(LogLevel.Debug, message, details);
        public void LogWarn(string message, object details = null) => LogWithLevel(LogLevel.Warning, message, details);
        public void LogTrace(string message, object details = null) => LogWithLevel(LogLevel.Trace, message, details);
        public void LogError(string message, Exception ex, object additionalDetails = null)
        {
            var errorDetails = new
            {
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,
                AdditionalDetails = additionalDetails
            };

            LogWithLevel(LogLevel.Error, message, errorDetails, ex);
        }

        public void SendLogToAzure(List<LogEntity> logEntities)
        {
            try
            {
                var logEvent = new LogEventInfo(NLog.LogLevel.Info, _nLogger.Name, string.Empty)
                {
                    Properties = { ["LogEntities"] = logEntities }
                };

                _nLogger.Log(logEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending logs to Azure");
            }
        }

        private void LogWithLevel(LogLevel level, string message, object details, Exception ex = null)
        {
            try
            {
                switch (level)
                {
                    case LogLevel.Information:
                        _logger.LogInformation(message);
                        break;
                    case LogLevel.Debug:
                        _logger.LogDebug(message);
                        break;
                    case LogLevel.Warning:
                        _logger.LogWarning(message);
                        break;
                    case LogLevel.Trace:
                        _logger.LogTrace(message);
                        break;
                    case LogLevel.Error:
                        _logger.LogError(ex, message);
                        break;
                }

                var logEvent = new LogEventInfo(level.ToNLogLevel(), _nLogger.Name, message)
                {
                    Exception = ex,
                    Properties = { ["Details"] = details != null ? JsonConvert.SerializeObject(details) : null }
                };

                //....................................................................... Create and log a LogEntity
                var logEntity = CreateLogEntity(logEvent);
                _nLogger.Log(logEvent);
            }
            catch (Exception logEx)
            {
                _logger.LogError(logEx, "Error during logging");
            }
        }

        public LogEntity CreateLogEntity(LogEventInfo logEvent)
        { 
            return new LogEntity
            {               
                Message = _layout.Render(logEvent),
                Level = logEvent.Level.Name,
                Timestamp = logEvent.TimeStamp.ToUniversalTime(),
                Detail = logEvent.Properties.ContainsKey("Details")
                    ? logEvent.Properties["Details"]?.ToString()
                    : null
            };
        }

    }

    //................................................................................... Class convert Microsoft LogLevel to NLog LogLevel.
    static class LogLevelExtensions
    {
        //............................................................................... Convert Microsoft LogLevel to NLog LogLevel.
        public static NLog.LogLevel ToNLogLevel(this LogLevel level) => level switch
        {
            LogLevel.Trace => NLog.LogLevel.Trace,
            LogLevel.Debug => NLog.LogLevel.Debug,
            LogLevel.Information => NLog.LogLevel.Info,
            LogLevel.Warning => NLog.LogLevel.Warn,
            LogLevel.Error => NLog.LogLevel.Error,
            _ => NLog.LogLevel.Info
        };
    }
}