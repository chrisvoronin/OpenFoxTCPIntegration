using System;
using System.Configuration;

namespace OpenFoxConsoleApp
{
    public class AppConfig
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public bool IsBigEndian { get; set; }
        public int MinReconnectSeconds { get; set; }
        public string MinLogLevelString { get; set; }
        public bool IsTest { get; set; }
        public string TestFileName { get; set; }
        public string DBConnectionString { get; set; }

        public AppConfig Load(AppSettingsReader reader)
        {
            return new AppConfig
            {
                IP = ReadSetting<string>(reader, "ip"),
                Port = ReadSetting<int>(reader, "port"),
                IsBigEndian = ReadSetting<bool>(reader, "isBigEndian"),
                MinReconnectSeconds = ReadSetting<int>(reader, "minReconnectSeconds"),
                MinLogLevelString = ReadSetting<string>(reader, "minLogLevel"),
                IsTest = ReadSetting<bool>(reader, "isTest"),
                TestFileName = ReadSetting<string>(reader, "testMessageFileName"),
                DBConnectionString = ReadSetting<string>(reader, "connectionString")
            };
        }

        private T ReadSetting<T>(AppSettingsReader reader, string key) =>
            (T)reader.GetValue(key, typeof(T));
    }
}

