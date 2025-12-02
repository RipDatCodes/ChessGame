namespace RoguesGambitServer.Core.Models
{
    /// <summary>
    /// High-level lobby lifecycle state.
    /// </summary>
    public enum LobbyStatus
    {
        WaitingForOpponent = 0,
        InProgress = 1,
        Completed = 2
    }
}
