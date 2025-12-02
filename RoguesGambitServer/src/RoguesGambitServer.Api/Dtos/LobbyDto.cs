using System;
using System.Collections.Generic;

namespace RoguesGambitServer.Api.Dtos
{
    public sealed class LobbyDto
    {
        public Guid LobbyId { get; set; }

        public string HostPlayerId { get; set; } = string.Empty;

        public string HostDisplayName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Visibility { get; set; } = string.Empty;

        public GameSettingsDto GameSettings { get; set; } = new();

        public ConnectionInfoDto ConnectionInfo { get; set; } = new();

        public List<PlayerSummaryDto> Players { get; set; } = new();

        public int MaxPlayers { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime LastHeartbeatAtUtc { get; set; }
    }
}
