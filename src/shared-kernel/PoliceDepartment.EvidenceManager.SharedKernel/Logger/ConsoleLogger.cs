using Microsoft.Extensions.Logging;

namespace PoliceDepartment.EvidenceManager.SharedKernel.Logger
{
    public class ConsoleLogger : ILoggerManager
    {
        private readonly ILogger<ConsoleLogger> _logger;

        public ConsoleLogger(ILogger<ConsoleLogger> logger)
        {
            _logger = logger;
        }

        public void LogDebug(string message, params (string name, object value)[] parameters)
        {
            Log(message, LoggerManagerSeverity.DEBUG, parameters);
        }

        public void LogInformation(string message, params (string name, object value)[] parameters)
        {
            Log(message, LoggerManagerSeverity.INFORMATION, parameters);
        }

        public void LogWarning(string message, params (string name, object value)[] parameters)
        {
            Log(message, LoggerManagerSeverity.WARNING, parameters);
        }

        public void LogError(string message, Exception exception = default, params (string name, object value)[] parameters)
        {
            var newParameters = new (string name, object value)[parameters.Length + 1];

            parameters.CopyTo(newParameters, 0);

            if (exception != null)
                newParameters[newParameters.Length + 1] = (nameof(exception), exception);

            Log(message, LoggerManagerSeverity.ERROR, newParameters);
        }

        public void LogCritical(string message, Exception exception = default, params (string name, object value)[] parameters)
        {
            var newParameters = new (string name, object value)[parameters.Length + 1];

            parameters.CopyTo(newParameters, 0);

            if (exception != null)
                newParameters[newParameters.Length + 1] = (nameof(exception), exception);

            Log(message, LoggerManagerSeverity.CRITICAL, newParameters);
        }

        public void Log(string message, LoggerManagerSeverity severity, params (string name, object value)[] parameters)
        {
            switch (severity)
            {
                case LoggerManagerSeverity.INFORMATION:
                    _logger.LogInformation(message, parameters);
                    break;
                case LoggerManagerSeverity.WARNING:
                    _logger.LogWarning(message, parameters);
                    break;
                case LoggerManagerSeverity.ERROR:
                    _logger.LogError(message, parameters);
                    break;
                case LoggerManagerSeverity.CRITICAL:
                    _logger.LogCritical(message, parameters);
                    break;
                default:
                    _logger.LogDebug(message, parameters);
                    break;
            }
        }
    }
}
