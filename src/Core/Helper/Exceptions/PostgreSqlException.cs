using System;

namespace Helper.Exceptions
{
    public class PostgreSqlException : Exception
    {
        public string Code { get; }
        
        public PostgreSqlException(string code, string message, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }
    }
}