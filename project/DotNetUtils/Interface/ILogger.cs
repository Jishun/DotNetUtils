namespace DotNetUtils
{
    public interface ILogger
    {
        void Log(string pattern, params object[] args);
        void LogError(string subcategory, string code, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, string helpKeyword, string senderName);
        void LogError(string subcategory, string code, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message);
        void LogError(string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message);
        void LogError(string pattern, params object[] args);
        void LogWarning(string pattern, params object[] args);
        void Trace(string pattern, params object[] args);

        bool Clear();
        bool IsEmpty();
        bool IsNullOrEmpty();
    }

}
