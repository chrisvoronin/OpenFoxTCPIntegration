namespace OpenFox.Logging
{
    public interface ILogger
    {
        void Error(string message);
        void Warning(string message);
        void Info(string message);
        void Verbose(string message);

    }
}
