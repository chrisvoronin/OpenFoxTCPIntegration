using System;
using System.Configuration;
using System.IO;
using System.Threading;
using OpenFox;
using OpenFox.DataAccess;
using OpenFox.Logging;

namespace OpenFoxConsoleApp
{
    internal class Program
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        static OpenFoxClientService _service;

        static void Main(string[] args)
        {
            var c = new AppConfig();
            c.Load(new AppSettingsReader());

            ILogger logger = LogFactory.CreateLogger(c);
            IMessageQueue queue = QueueFactory.CreateQueue(c, logger);

            _service = new OpenFoxClientService(c.IP, c.Port, queue, logger, c.MinReconnectSeconds, c.IsBigEndian);
            _service.Start();

            _quitEvent.WaitOne(); // wait forever
        }

        /// <summary>
        /// Factory to create logging, for now leaving only console logging.
        /// </summary>
        private static class LogFactory
        {
            public static ILogger CreateLogger(AppConfig config)
            {
                LogLevel minLogLevel;
                if (!Enum.TryParse(config.MinLogLevelString, out minLogLevel))
                {
                    minLogLevel = LogLevel.Info;
                }

                ILogger logger = new ConsoleLogger(minLogLevel);
                return logger;
            }
        }

        /// <summary>
        /// Factory for making test or production version of the queue.
        /// </summary>
        private static class QueueFactory
        {
            public static IMessageQueue CreateQueue(AppConfig config, ILogger logger)
            {
                return config.IsTest ? CreateTestQueue(config, logger) : CreateProdQueue(config, logger);
            }

            private static IMessageQueue CreateTestQueue(AppConfig config, ILogger logger)
            {
                string fileName = config.TestFileName;
                string message = File.Exists(fileName) ? File.ReadAllText(fileName) : null;
                return new MessageQueueTest(logger, message);
            }

            private static IMessageQueue CreateProdQueue(AppConfig config, ILogger logger)
            {
                return new MessageQueueProd(config.DBConnectionString, logger);
            }
        }
    }
}
