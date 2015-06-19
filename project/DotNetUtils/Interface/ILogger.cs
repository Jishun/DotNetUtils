namespace DotNetUtils.Interface
{
    public interface ILogger
    {
        void Log(string pattern, params object[] args);
        void LogError(string pattern, params object[] args);
        void LogWarning(string pattern, params object[] args);
        void Trace(string pattern, params object[] args);
    }

}
