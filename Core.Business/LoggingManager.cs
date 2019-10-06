//using System;
//using System.Threading.Tasks;
//using CoreServices.Shared.Logging.Proto;
//using Microsoft.Extensions.Logging;
//using LogLevel = CoreServices.Shared.Logging.Proto.LogLevel;
//using MsLogging = Microsoft.Extensions.Logging;
//
//namespace CoreServices.Business.Logging
//{
//    public class LoggingManager
//    {
//        private readonly ILogger<LoggingManager> _logger;
//
//        public LoggingManager(ILogger<LoggingManager> logger)
//        {
//            _logger = logger;
//        }
//
//        public virtual LoggingCapabilities GetCapabilities()
//        {
//            return new LoggingCapabilities
//            {
//                AllowDebug = _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug),
//                AllowInfo = _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information),
//                AllowWarn = _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning),
//                AllowError = _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error),
//                AllowCritical = _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Critical),
//            };
//        }
//
//        public virtual Task LogAsync(MsLogging.LogLevel logLevel, CoreLoggingEvent loggingEvent)
//        {
//            _logger.Log(logLevel, default(EventId), loggingEvent, (Exception)null, CoreLoggingEvent.Formatter);
//            return Task.CompletedTask;
//        }
//
//        public static MsLogging.LogLevel ConvertLogLevel(LogLevel level)
//        {
//            var newLevel = MsLogging.LogLevel.None;
//            switch (level)
//            {
//                case LogLevel.Debug:
//                    newLevel = MsLogging.LogLevel.Debug;
//                    break;
//                case LogLevel.Info:
//                    newLevel = MsLogging.LogLevel.Information;
//                    break;
//                case LogLevel.Warn:
//                    newLevel = MsLogging.LogLevel.Warning;
//                    break;
//                case LogLevel.Error:
//                    newLevel = MsLogging.LogLevel.Error;
//                    break;
//                case LogLevel.Critical:
//                    newLevel = MsLogging.LogLevel.Critical;
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
//            }
//
//            return newLevel;
//        }
//    }
//}