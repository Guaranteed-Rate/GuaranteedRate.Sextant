using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Exceptions
{
    /// <summary>
    /// Represents an exception related to logging into a server
    /// </summary>
    public class ServerLoginException : Exception
    {
        public ErrorTypes ErrorType { get; private set; }

        public ServerLoginException(string message) : base(message) { this.ErrorType = ErrorTypes.Unspecified; }

        public ServerLoginException(string message, Exception innerException) : base(message, innerException) { this.ErrorType = ErrorTypes.Unspecified; }

        public ServerLoginException(string message, Exception innerException, ErrorTypes errorType) : base(message, innerException) { this.ErrorType = errorType; }


        /// <summary>
        /// List of known error types, for now, mapped from: EllieMae.Encompass.Client.LoginErrorType
        /// </summary>
        public enum ErrorTypes
        {
            Unspecified = 0,
            UserNotFound = 1,
            InvalidPassword = 2,
            UserDiabled = 4,
            LoginsDisabled = 5,
            ServerError = 6,
            UserLocked = 7,
            InvalidPersona = 8,
            ConcurrentEditingOfflineNotAllowed = 9,
            IPBlocked = 10,
            ServerBusy = 12,
            APIUserRestricted = 13
        }
    }
}
