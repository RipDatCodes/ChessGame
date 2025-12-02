namespace RoguesGambitServer.Core.Errors
{
    /// <summary>
    /// Domain-specific error codes used by the lobby service.
    /// These map directly to API error responses.
    /// </summary>
    public enum LobbyErrorCode
    {
        Unknown = 0,
        LobbyNotFound = 1,
        NotHost = 2,
        LobbyFull = 3,
        LobbyNotJoinable = 4,
        InvalidRequest = 5,
        ConcurrencyConflict = 6
    }
}
