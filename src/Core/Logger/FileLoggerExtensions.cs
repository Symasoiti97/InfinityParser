using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Logger
{
    public static class FileLoggerExtensions
    {
        public static void AddFileProvider(this ILoggingBuilder builder)
        {
            var location = Assembly.GetEntryAssembly()?.Location;
            
            var directory = Path.GetDirectoryName(location);
            if (directory == null) throw new FileNotFoundException();

            var fileName = Path.GetFileName(location).Split('.').First();

            var directoryLog = Path.Combine(directory, "Log");
            if (!Directory.Exists(directoryLog))
            {
                Directory.CreateDirectory(directoryLog);
            }

            builder.AddProvider(new FileLoggerProvider(Path.Combine(directoryLog, $"{fileName}.log")));
        }
    }
}