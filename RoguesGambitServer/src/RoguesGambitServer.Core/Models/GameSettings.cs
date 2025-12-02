namespace RoguesGambitServer.Core.Models
{
    /// <summary>
    /// Chess-specific game configuration.
    /// </summary>
    public sealed class GameSettings
    {
        /// <summary>
        /// Time control in "minutes+incrementSeconds" form, e.g. "10+0", "5+5".
        /// </summary>
        public string TimeControl { get; init; } = "10+0";

        /// <summary>
        /// Variant identifier, e.g. "standard", "blitz", "rapid", "chess960".
        /// </summary>
        public string Variant { get; init; } = "standard";

        /// <summary>
        /// Whether the game is rated or casual.
        /// </summary>
        public bool Rated { get; init; }
    }
}
