using LogAnalytics.Client;
using NLog;
using NLog.Targets;
using NLogWebAPI.Models;

namespace NLogWebAPI.LogTarget
{
    [Target("LogAnalytics")]
    public class LogAnalyticsTarget : TargetWithLayout
    {
        public string CustomerId { get; set; }
        public string SharedKey { get; set; }
        public string LogType { get; set; }

        private LogAnalyticsClient _client;

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            _client = new LogAnalyticsClient(CustomerId, SharedKey);
        }

        protected override async void Write(LogEventInfo logEvent)
        {
            if (logEvent.Properties.ContainsKey("LogEntities") && logEvent.Properties["LogEntities"] is List<LogEntity> logEntity)
            {
                _client.SendLogEntries(logEntity, LogType);

            }


        }
    }
}
