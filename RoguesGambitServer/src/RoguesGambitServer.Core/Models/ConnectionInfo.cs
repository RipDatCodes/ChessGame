using System.Collections.Generic;

namespace RoguesGambitServer.Core.Models
{
    /// <summary>
    /// Describes how a client should connect to the actual chess game host.
    /// This is intentionally generic so it can represent direct IP, relay, or dedicated server.
    /// </summary>
    public sealed class ConnectionInfo
    {
        /// <summary>
        /// Connection mode: e.g. "direct", "relay", "dedi".
        /// </summary>
        public string Mode { get; init; } = "direct";

        /// <summary>
        /// Address or join code used by the client to connect.
        /// For direct mode this might be "203.0.113.42:7777".
        /// For relay mode this might be a join code or room id.
        /// </summary>
        public string Address { get; init; } = string.Empty;

        /// <summary>
        /// Optional extra metadata for the networking layer.
        /// </summary>
        public Dictionary<string, string> Metadata { get; init; } = new();
    }
}
