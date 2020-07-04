using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Logger
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath;
        private static readonly object Lock = new object();

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.Information;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                lock (Lock)
                {
                    File.AppendAllText(_filePath, $"[{DateTime.UtcNow}]\t{logLevel}: " + formatter(state, exception) + Environment.NewLine);
                }
            }
        }
    }
}