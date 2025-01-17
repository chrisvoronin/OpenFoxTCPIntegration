using System;

namespace OpenFox.Logging
{
    public enum LogLevel
    {
        Verbose = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    public class ConsoleLogger : ILogger
    {
        private readonly LogLevel _minimumLogLevel;

        public ConsoleLogger(LogLevel minLogLevel)
        {
            _minimumLogLevel = minLogLevel;
        }

        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        public void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }
        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        public void Verbose(string message)
        {
            Log(LogLevel.Verbose, message);
        }

        public void Log(LogLevel level, string message)
        {
            if (level >= _minimumLogLevel)
            {
                Console.WriteLine($"{DateTime.Now}");
                Console.WriteLine(message);
            }
        }
    }
}
