using System;

namespace GuaranteedRate.Sextant.Exceptions
{
    /// <summary>
    /// Represents an exception related to a Server Connection
    /// </summary>
    public class ServerConnectionException : Exception
    {
        public ServerConnectionException(string message) : base(message) { }

        public ServerConnectionException(string message, Exception innerException) : base(message, innerException) { }

    }
}
