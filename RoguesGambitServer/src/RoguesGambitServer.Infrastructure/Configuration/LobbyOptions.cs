namespace RoguesGambitServer.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration for lobby heartbeat timeout and cleanup interval.
    /// Values are in seconds.
    /// </summary>
    public sealed class LobbyOptions
    {
        /// <summary>
        /// How long a lobby can go without a heartbeat before it is considered expired.
        /// Default: 30 seconds.
        /// </summary>
        public int HeartbeatTimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// How often the cleanup job runs.
        /// Default: 10 seconds.
        /// </summary>
        public int CleanupIntervalSeconds { get; set; } = 10;
    }
}
