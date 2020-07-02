using System;

namespace Helper.Exceptions
{
    public class ObjectAlreadyExistsException : Exception
    {
        public ObjectAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}