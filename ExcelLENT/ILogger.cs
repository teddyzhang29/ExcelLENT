namespace BBGo.ExcelLENT
{
    public interface ILogger
    {
        void Log(object msg);
        void LogWarning(object msg);
        void LogError(object msg);
    }
}