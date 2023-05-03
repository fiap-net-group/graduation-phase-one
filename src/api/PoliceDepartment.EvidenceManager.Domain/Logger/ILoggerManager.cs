namespace PoliceDepartment.EvidenceManager.Domain.Logger
{
    public interface ILoggerManager
    {
        void LogInformation(string message, params (string name, object value)[] parameters);
        void LogWarning(string message, params (string name, object value)[] parameters);
        void LogError(string message, Exception exception = default, params (string name, object value)[] parameters);
        void LogDebug(string message, params (string name, object value)[] parameters);
        void Log(string message, LoggerManagerSeverity severity, params (string name, object value)[] parameters);
    }
}
