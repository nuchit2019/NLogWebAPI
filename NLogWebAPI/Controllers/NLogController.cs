using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using NLogWebAPI.Models;
using NLogWebAPI.Services;

namespace NLogWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NLogController : ControllerBase
    {
        private readonly ILoggerService _loggerService;
        private readonly Logger _nLogger;

        public NLogController(ILoggerService loggerService)
        {
            _loggerService = loggerService;
            _nLogger = LogManager.GetCurrentClassLogger(); // Initialize NLog logger
        }

        [HttpGet("write-log")]
        public IActionResult WriteLog()
        { 

            _loggerService.LogInfo("START =============================");

            List<LogEntity> lsLogEntity = new List<LogEntity>();

            try
            {
                // ================================== LogInfo ================================== //
                var detail = new  { UserId = "1234", UserName = "john_doe", Email = "john.doe@example.com" };
                lsLogEntity.Add(new LogEntity
                {
                    Message = "1.[TEST_LogInfo]...",
                    Level = NLog.LogLevel.Info.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Detail = JsonConvert.SerializeObject(detail)
                });
                //_loggerService.LogInfo("1.[TEST_LogInfo] This is an information log.", detail);
                _loggerService.LogInfo("1.[TEST_LogInfo] This is an information log.");

                // ================================== LogWarn ================================== //
                lsLogEntity.Add(new LogEntity
                {
                    Message = "2.[TEST_LogWarn]...",
                    Level = NLog.LogLevel.Warn.ToString(),
                    Timestamp = DateTime.UtcNow ,
                    Detail = "2.[TEST_LogWarn]..."
                });
                _loggerService.LogWarn("2.[TEST_LogWarn] This is an LogWarn log.");

                // ================================== LogDebug ================================== //
                lsLogEntity.Add(new LogEntity
                {
                    Message = "3.[TEST_LogDebug]...",
                    Level = NLog.LogLevel.Info.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Detail = "3.[TEST_LogDebug]..."
                });
                _loggerService.LogDebug("3.[TEST_LogDebug] This is an LogDebug log.");

                // ================================== LogTrace ================================== //
                lsLogEntity.Add(new LogEntity
                {
                    Message = "4.[TEST_LogTrace]...",
                    Level = NLog.LogLevel.Info.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Detail = "4.[TEST_LogTrace]..."
                });
                _loggerService.LogTrace("4.[TEST_LogTrace] This is an LogTrace log.");

                ///==============================
                ///
                throw new Exception("Test exception for logging.");
            }
            catch (Exception ex)
            {

                // ================================== LogError ================================== //
                var ERR = new
                {
                    ex.Message,
                    ex.StackTrace,
                    Note = "Error encountered during logging"
                };

                // Add error log entity
                lsLogEntity.Add(new LogEntity
                {
                    Message = "5.[TEST_LogTrace]...An ERROR occurred during logging",
                    Level = NLog.LogLevel.Error.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Detail = JsonConvert.SerializeObject(ERR)
                });

                _loggerService.LogError("5.[TEST_LogTrace] An error occurred during logging.", ex, ERR);
            }
            finally
            {
                // ================================== SendLogToAzure ================================== //
                if (lsLogEntity.Count > 0)
                {
                    _loggerService.SendLogToAzure(lsLogEntity);
                }
                _loggerService.LogInfo("SendLogToAzure.........");
                _loggerService.LogInfo("END =============================");
                
            }

            return Ok("Logs have been written...............");
        }


        [HttpGet("SendLogToAzure")]
        public IActionResult TestSendLogToAzure()
        {
            List<LogEntity> lsLogEntity = new List<LogEntity>(); 

            _loggerService.LogInfo("START =============================");

            try
            {
                // ================================== LogInfo ================================== //
                var infoDetail = new { UserId = "1234", UserName = "john_doe", Email = "john.doe@example.com" };
                var infoLogEvent = new LogEventInfo(NLog.LogLevel.Info, _nLogger.Name, "1.[TEST_LogInfo] This is an information log.")
                {
                    Properties = { ["Detail"] = JsonConvert.SerializeObject(infoDetail) }
                };
                lsLogEntity.Add(_loggerService.CreateLogEntity(infoLogEvent));
                _loggerService.LogInfo(infoLogEvent.Message, infoDetail);

                // ================================== LogWarn ================================== //
                var warnLogEvent = new LogEventInfo(NLog.LogLevel.Warn, _nLogger.Name, "2.[TEST_LogWarn] This is a warning log.");
                lsLogEntity.Add(_loggerService.CreateLogEntity(warnLogEvent));
                _loggerService.LogWarn(warnLogEvent.Message);

                // ================================== LogDebug ================================== //
                var debugLogEvent = new LogEventInfo(NLog.LogLevel.Debug, _nLogger.Name, "3.[TEST_LogDebug] This is a debug log.");
                lsLogEntity.Add(_loggerService.CreateLogEntity(debugLogEvent));
                _loggerService.LogDebug(debugLogEvent.Message);

                // ================================== LogTrace ================================== //
                var traceLogEvent = new LogEventInfo(NLog.LogLevel.Trace, _nLogger.Name, "4.[TEST_LogTrace] This is a trace log.");
                lsLogEntity.Add(_loggerService.CreateLogEntity(traceLogEvent));
                _loggerService.LogTrace(traceLogEvent.Message);

                // Simulate an exception
                throw new Exception("Test exception for logging.");
            }
            catch (Exception ex)
            {
                // ================================== LogError ================================== //
                var errorDetails = new
                {
                    ex.Message,
                    ex.StackTrace,
                    Note = "Error encountered during logging"
                };

                var errorLogEvent = new LogEventInfo(NLog.LogLevel.Error, _nLogger.Name, "5.[TEST_LogError] An ERROR occurred during logging")
                {
                    Properties = { ["Detail"] = JsonConvert.SerializeObject(errorDetails) },
                    Exception = ex
                };

                lsLogEntity.Add(_loggerService.CreateLogEntity(errorLogEvent));
                _loggerService.LogError(errorLogEvent.Message, ex, errorDetails);
            }
            finally
            {
                // ================================== SendLogToAzure ================================== //
                if (lsLogEntity.Count > 0)
                {
                    _loggerService.SendLogToAzure(lsLogEntity);
                }
                _loggerService.LogInfo("SendLogToAzure completed.");
                _loggerService.LogInfo("END =============================");
                 
            }

            return Ok("Logs have been written successfully.");
        }



    }
}
