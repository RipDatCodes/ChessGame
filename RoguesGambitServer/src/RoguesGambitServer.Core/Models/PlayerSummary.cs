namespace RoguesGambitServer.Core.Models
{
    /// <summary>
    /// Minimal information about a player in a lobby.
    /// </summary>
    public sealed class PlayerSummary
    {
        public string PlayerId { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;
    }
}
