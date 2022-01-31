using System;

namespace Db.Exceptions
{
    public class PostgresSqlException : Exception
    {
        public string Code { get; }

        public PostgresSqlException(string code, string message, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }
    }
}