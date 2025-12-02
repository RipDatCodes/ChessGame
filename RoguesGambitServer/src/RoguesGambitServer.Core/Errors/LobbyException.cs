using System;
using RoguesGambitServer.Core.Errors;

namespace RoguesGambitServer.Core.Exceptions
{
    /// <summary>
    /// Exception type used inside the domain/service layer to signal
    /// expected business rule violations that should become structured errors at the API boundary.
    /// </summary>
    public class LobbyException : Exception
    {
        public LobbyErrorCode ErrorCode { get; }

        public LobbyException(LobbyErrorCode errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public LobbyException(LobbyErrorCode errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
