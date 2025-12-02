using System;
using System.Collections.Generic;

namespace RoguesGambitServer.Core.Models
{
    /// <summary>
    /// Represents a single hosted game lobby.
    /// </summary>
    public class Lobby
    {
        public Guid LobbyId { get; set; }

        public string HostPlayerId { get; set; } = string.Empty;

        public string HostDisplayName { get; set; } = string.Empty;

        public LobbyStatus Status { get; set; } = LobbyStatus.WaitingForOpponent;

        public LobbyVisibility Visibility { get; set; } = LobbyVisibility.Public;

        public GameSettings GameSettings { get; set; } = new GameSettings();

        public ConnectionInfo ConnectionInfo { get; set; } = new ConnectionInfo();

        /// <summary>
        /// Players currently associated with this lobby.
        /// For chess this will typically be 1 or 2 players.
        /// </summary>
        public List<PlayerSummary> Players { get; } = new List<PlayerSummary>();

        /// <summary>
        /// Maximum number of players allowed in this lobby.
        /// For chess this will be 2.
        /// </summary>
        public int MaxPlayers { get; set; } = 2;

        /// <summary>
        /// UTC timestamp when the lobby was created.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// UTC timestamp of the most recent heartbeat from the host.
        /// Used for timeout-based cleanup.
        /// </summary>
        public DateTime LastHeartbeatAtUtc { get; set; }
    }
}
